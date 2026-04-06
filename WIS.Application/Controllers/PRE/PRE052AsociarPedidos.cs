using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Security;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRE
{
    public class PRE052AsociarPedidos : AppController
    {
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE052AsociarPedidos(
            ISecurityService security,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IIdentityService identity,
            IGridExcelService excelService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._security = security;
            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._identity = identity;
            this._excelService = excelService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idPrep = query.GetParameter("keyPreparacion");

            if (!string.IsNullOrEmpty(idPrep))
            {
                var preparacion = uow.PreparacionRepository.GetPreparacionPorNumero(int.Parse(idPrep));

                if (preparacion == null)
                    throw new ValidationFailedException("PRE052_Sec0_Error_PreparacionNoExiste", new string[] { idPrep });

                query.AddParameter("infoPrep", $"{preparacion.Id}");
            }

            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext query)
        {
            if (query.ButtonId == "btnCerrar")
                this._concurrencyControl.ClearToken();

            return base.FormButtonAction(form, query);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (this._security.IsUserAllowed(SecurityResources.WPRE052_grid1_btn_AsociarPedidos))
            {
                if (grid.Id == "AgregarPedido_grid_1")
                {
                    grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_Agregar"));
                }
                else if (grid.Id == "QuitarPedido_grid_2")
                {
                    grid.MenuItems.Add(new GridButton("btnQuitar", "General_Sec0_btn_Quitar"));
                }
            }

            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idPrep = int.Parse(context.GetParameter("preparacion"));
            var emp = int.Parse(context.GetParameter("empresa"));

            if (grid.Id == "AgregarPedido_grid_1")
            {
                var dbQuery = new PRE052AsociarPedidosQuery(idPrep, emp);

                uow.HandleQuery(dbQuery);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            else if (grid.Id == "QuitarPedido_grid_2")
            {
                var dbQuery = new PRE052QuitarPedidosQuery(idPrep);

                uow.HandleQuery(dbQuery);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    var pedido = row.GetCell("NU_PEDIDO").Value;
                    var cliente = row.GetCell("CD_CLIENTE").Value;
                    var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

                    if (!uow.PreparacionRepository.PuedoDesasociarPedido(idPrep, pedido, cliente, empresa))
                        row.DisabledSelected = true;
                }
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idPrep = int.Parse(query.GetParameter("preparacion"));
            var emp = int.Parse(query.GetParameter("empresa"));

            if (grid.Id == "AgregarPedido_grid_1")
            {
                var dbQuery = new PRE052AsociarPedidosQuery(idPrep, emp);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "QuitarPedido_grid_2")
            {
                var dbQuery = new PRE052QuitarPedidosQuery(idPrep);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idPrep = int.Parse(context.GetParameter("preparacion"));
            var emp = int.Parse(context.GetParameter("empresa"));

            if (grid.Id == "AgregarPedido_grid_1")
            {
                var dbQuery = new PRE052AsociarPedidosQuery(idPrep, emp);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else if (grid.Id == "QuitarPedido_grid_2")
            {
                var dbQuery = new PRE052QuitarPedidosQuery(idPrep);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }

            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "AgregarPedido_grid_1" && context.ButtonId == "btnAgregar")
            {
                this.ProcesarAgregar(context);
            }
            else if (context.GridId == "QuitarPedido_grid_2" && context.ButtonId == "btnQuitar")
            {
                this.ProcesarQuitar(context);
            }

            return context;
        }

        #region Auxs

        public virtual void ProcesarAgregar(GridMenuItemActionContext context)
        {
            var idPrep = int.Parse(context.GetParameter("preparacion"));
            var emp = int.Parse(context.GetParameter("empresa"));

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("PRE052 Crear Preparacion manual por Pedidos");
            uow.BeginTransaction();

            var transaccion = uow.GetTransactionNumber();

            try
            {
                var pedidos = new List<Pedido>();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new PRE052AsociarPedidosQuery(idPrep, emp);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    pedidos = dbQuery.GetPedidos();
                }
                else
                {
                    var selection = context.Selection.GetSelection(this.GridKeys);

                    pedidos = selection.Select(item => new Pedido
                    {
                        Id = item["NU_PEDIDO"],
                        Empresa = int.Parse(item["CD_EMPRESA"]),
                        Cliente = item["CD_CLIENTE"],
                    }).ToList();
                }

                AsociarPedidos(uow, pedidos, idPrep, transaccion);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("PRE052_Sucess_msg_PedidosAsociados");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }
        }

        public virtual void ProcesarQuitar(GridMenuItemActionContext context)
        {
            var idPrep = int.Parse(context.GetParameter("preparacion"));

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("PRE052 Crear Preparacion manual por Pedidos");
            uow.BeginTransaction();

            try
            {
                var pedidos = new List<Pedido>();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new PRE052QuitarPedidosQuery(idPrep);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    pedidos = dbQuery.GetPedidos();
                }
                else
                {
                    var selection = context.Selection.GetSelection(this.GridKeys);

                    pedidos = selection.Select(item => new Pedido
                    {
                        Id = item["NU_PEDIDO"],
                        Empresa = int.Parse(item["CD_EMPRESA"]),
                        Cliente = item["CD_CLIENTE"],
                    }).ToList();
                }

                DesasociarPedidos(uow, pedidos, idPrep, out bool ok);

                uow.SaveChanges();
                uow.Commit();

                if (ok)
                    context.AddSuccessNotification("PRE052_Sucess_msg_PedidosDesasociados");
                else
                    context.AddSuccessNotification("PRE052_Sucess_msg_PedidosDesasociadosParcial");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }
        }

        public virtual void AsociarPedidos(IUnitOfWork uow, List<Pedido> pedidos, int prep, long? transaccion)
        {
            foreach (var ped in pedidos)
            {
                var pedido = uow.PedidoRepository.GetPedido(ped.Empresa, ped.Cliente, ped.Id);

                pedido.Transaccion = transaccion;
                pedido.NroPrepManual = prep;
                pedido.FechaModificacion = DateTime.Now;
                pedido.Transaccion = uow.GetTransactionNumber();

                uow.PedidoRepository.UpdatePedido(pedido);

                if (pedido.NuCarga == null)
                {
                    var carga = new Carga
                    {
                        Descripcion = "Generada por la preparación manual: " + prep,
                        Preparacion = prep,
                        Ruta = (short)pedido.Ruta,
                        FechaAlta = DateTime.Now
                    };

                    uow.CargaRepository.AddCarga(carga);
                }
            }
        }

        public virtual void DesasociarPedidos(IUnitOfWork uow, List<Pedido> pedidos, int prep, out bool ok)
        {
            ok = true;
            foreach (var ped in pedidos)
            {
                if (uow.PreparacionRepository.PuedoDesasociarPedido(prep, ped.Id, ped.Cliente, ped.Empresa))
                {
                    var pedido = uow.PedidoRepository.GetPedido(ped.Empresa, ped.Cliente, ped.Id);

                    pedido.Transaccion = uow.GetTransactionNumber();
                    pedido.NroPrepManual = null;
                    pedido.FechaModificacion = DateTime.Now;
                    uow.PedidoRepository.UpdatePedido(pedido);
                }
                else
                    ok = false;
            }
        }

        #endregion
    }
}
