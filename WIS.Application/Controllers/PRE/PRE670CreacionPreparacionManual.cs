using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE670CreacionPreparacionManual : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public PRE670CreacionPreparacionManual(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_EMPRESA", "CD_CLIENTE", "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_FAIXA"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            grid.MenuItems = new List<IGridItem> {
                    new GridButton("btnGuardar", "General_Sec0_btn_RestoreValorPorDefecto"),
                };

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<string[]> keysRowSelected = this.GetSelectedPlanificaciones(uow, context);

            foreach (var detselect in keysRowSelected)
            {
                var pedido = uow.PedidoRepository.GetPedido(int.Parse(detselect[1]), detselect[2], detselect[0]);

                DetallePedido det = uow.PedidoRepository.GetDetallePedido(pedido.Id, pedido.Empresa, pedido.Cliente, detselect[3], detselect[4], decimal.Parse(detselect[5], _identity.GetFormatProvider()), detselect[6]);
                det.Cantidad = det.CantidadOriginal;
                uow.PedidoRepository.UpdateDetallePedido(det);
            }

            uow.SaveChanges();

            return context;
        }

        public virtual List<string[]> GetSelectedPlanificaciones(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new PlanificacionDeCantidadesLiberarQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());

            return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_PEDIDO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            PlanificacionDeCantidadesLiberarQuery dbQuery = null;

            dbQuery = new PlanificacionDeCantidadesLiberarQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);
            grid.SetEditableCells(new List<string> { "QT_PEDIDO" });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PlanificacionDeCantidadesLiberarQuery dbQuery = null;

            dbQuery = new PlanificacionDeCantidadesLiberarQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_PEDIDO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            PlanificacionDeCantidadesLiberarQuery dbQuery = null;

            dbQuery = new PlanificacionDeCantidadesLiberarQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string fl = context.GetParameter("FL_AJUSTAR_DETALLE");
            List<DetallePedido> detalles_modificados = new List<DetallePedido>();

            grid.Rows.ForEach(row =>
            {
                decimal.TryParse(row.GetCell("QT_ANULADO").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal qtanu);
                decimal.TryParse(row.GetCell("QT_LIBERADO").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal qtlib);
                decimal.TryParse(row.GetCell("QT_PEDIDO").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal qtpedido);

                var pedido = uow.PedidoRepository.GetPedido(int.Parse(row.GetCell("CD_EMPRESA").Value), row.GetCell("CD_CLIENTE").Value, row.GetCell("NU_PEDIDO").Value);

                DetallePedido detallePedido = uow.PedidoRepository.GetDetallePedido(pedido.Id, pedido.Empresa, pedido.Cliente,
                    row.GetCell("CD_PRODUTO").Value, row.GetCell("NU_IDENTIFICADOR").Value, decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider()), row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value);

                detallePedido.Cantidad = qtpedido;

                uow.PedidoRepository.UpdateDetallePedido(detallePedido);

                if (!string.IsNullOrEmpty(fl) && fl.Equals("S"))
                {
                    DetallePedido det = new DetallePedido();
                    det.Producto = row.GetCell("CD_PRODUTO").Value;
                    det.Identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                    det.Id = row.GetCell("NU_PEDIDO").Value;
                    det.Cliente = row.GetCell("CD_CLIENTE").Value;
                    det.Empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                    detalles_modificados.Add(det);
                }
            });

            int empresa = -1;
            List<Pedido> ListPedido = new List<Pedido>();

            foreach (var det in detalles_modificados)
            {
                if (empresa == -1)
                {
                    Pedido ped = new Pedido();
                    ped.Id = det.Id;
                    ped.Cliente = det.Cliente;
                    ped.Empresa = det.Empresa;
                    ListPedido.Add(ped);
                }
                else
                {
                    bool existe = false;
                    
                    foreach (var a in ListPedido)
                    {
                        if (a.Id == det.Id && a.Cliente == det.Cliente && a.Empresa == det.Empresa)
                        {
                            existe = true;
                            break;
                        }
                    }

                    if (!existe)
                    {
                        Pedido ped = new Pedido();
                        ped.Id = det.Id;
                        ped.Cliente = det.Cliente;
                        ped.Empresa = det.Empresa;
                        ListPedido.Add(ped);
                    }

                }
            }

            foreach (var ped in ListPedido)
            {
                List<DetallePedido> detallesPedido = uow.PedidoRepository.GetDetallesPedido(ped);

                foreach (var det in detallesPedido)
                {
                    if (!existeDetalleEnLista(detalles_modificados, det))
                    {
                        det.Cantidad = det.CantidadLiberada + det.CantidadAnulada;
                        uow.PedidoRepository.UpdateDetallePedido(det);
                    }
                }
            }

            uow.SaveChanges();

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRE670GridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public virtual bool existeDetalleEnLista(List<DetallePedido> detalles_modificados, DetallePedido det)
        {
            foreach (var a in detalles_modificados)
            {
                if (a.Id.Equals(det.Id) && a.Empresa == det.Empresa && a.Cliente.Equals(det.Cliente) && a.Producto.Equals(det.Producto) && a.Identificador.Equals(det.Identificador))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
