using System.Collections.Generic;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.GridComponent;
using WIS.PageComponent.Execution;
using WIS.Sorting;
using WIS.Domain.DataModel;
using WIS.Filtering;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Build;
using WIS.Security;
using System.Linq;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel.Configuration;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
using DocumentFormat.OpenXml.InkML;
using System;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100PedidoLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE100PedidoLpn(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "TP_LPN_TIPO", "ID_LPN_EXTERNO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override PageContext PageLoad(PageContext data)
        {
            return base.PageLoad(data);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "PRE100PedidoLpn_grid_1")
                grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_AgregarSeleccion"));
            else if (grid.Id == "PRE100PedidoLpn_grid_2")
                grid.MenuItems.Add(new GridButton("btnQuitar", "General_Sec0_btn_QuitarSeleccion"));

			using var uow = this._uowFactory.GetUnitOfWork();

			var pedidoId = context.GetParameter("pedido");
			var clienteId = context.GetParameter("cliente");
			var empresaId = int.Parse(context.GetParameter("empresa"));

			var empresa = uow.EmpresaRepository.GetEmpresa(empresaId);
			var cliente = uow.AgenteRepository.GetAgente(empresaId, clienteId);

			context.AddParameter("empresaNombre", empresa.Nombre);

			context.AddParameter("agenteDescripcion", cliente.Descripcion);
			context.AddParameter("agenteCodigo", cliente.Codigo);
			context.AddParameter("agenteTipo", cliente.Tipo);

			return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

            if (grid.Id == "PRE100PedidoLpn_grid_1")
            {
                var dbQuery = new PedidosLpnDisponiblesQuery(pedido);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            else if (grid.Id == "PRE100PedidoLpn_grid_2")
            {
                var dbQuery = new PedidosLpnAgregadosQuery(pedido);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    var qtLiberado = decimal.Parse(row.GetCell("QT_LIBERADO").Value, _identity.GetFormatProvider());
                    var qtAnulado = decimal.Parse(row.GetCell("QT_ANULADO").Value, _identity.GetFormatProvider());

                    if (qtLiberado > 0 || qtAnulado > 0)
                        row.DisabledSelected = true;
                }
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var empresa = int.Parse(query.GetParameter("empresa"));
            var cliente = query.GetParameter("cliente");
            var nuPedido = query.GetParameter("pedido");

            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

            if (grid.Id == "PRE100PedidoLpn_grid_1")
            {
                var dbQuery = new PedidosLpnDisponiblesQuery(pedido);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "PRE100PedidoLpn_grid_2")
            {
                var dbQuery = new PedidosLpnAgregadosQuery(pedido);

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

            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

            if (grid.Id == "PRE100PedidoLpn_grid_1")
            {
                var dbQuery = new PedidosLpnDisponiblesQuery(pedido);

                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else if (grid.Id == "PRE100PedidoLpn_grid_2")
            {
                var dbQuery = new PedidosLpnAgregadosQuery(pedido);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            try
            {
                var empresa = int.Parse(context.GetParameter("empresa"));
                var cliente = context.GetParameter("cliente");
                var nuPedido = context.GetParameter("pedido");

                using var uow = this._uowFactory.GetUnitOfWork();

                var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

                if (context.GridId == "PRE100PedidoLpn_grid_1" && context.ButtonId == "btnAgregar")
                    ProcesarAgregar(uow, context, pedido);
                else if (context.GridId == "PRE100PedidoLpn_grid_2" && context.ButtonId == "btnQuitar")
                    ProcesarQuitar(uow, context, pedido);

            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }

            return context;
        }

        public virtual void ProcesarAgregar(IUnitOfWork uow, GridMenuItemActionContext context, Pedido pedido)
        {
            uow.CreateTransactionNumber("PRE100PedidoLpn: Agregar");

            var lpns = new List<Lpn>();

            var dbQuery = new PedidosLpnDisponiblesQuery(pedido);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
            {
                lpns = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys)
                    .Select(l => new Lpn()
                    {
                        Tipo = l[3],
                        IdExterno = l[4]
                    })
                    .ToList();
            }
            else
            {
                lpns = dbQuery.GetSelectedKeys(context.Selection.Keys)
                    .Select(l => new Lpn()
                    {
                        Tipo = l[3],
                        IdExterno = l[4]
                    })
                    .ToList();
            }

            uow.PedidoRepository.AsociarLpn(pedido, lpns, uow.GetTransactionNumber());
        }

        public virtual void ProcesarQuitar(IUnitOfWork uow, GridMenuItemActionContext context, Pedido pedido)
        {
            uow.CreateTransactionNumber("PRE100PedidoLpn: Quitar");

            var lpns = new List<Lpn>();

            var dbQuery = new PedidosLpnAgregadosQuery(pedido);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
            {
                lpns = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys)
                    .Select(l => new Lpn()
                    {
                        Tipo = l[3],
                        IdExterno = l[4]
                    })
                    .ToList();
            }
            else
            {
                lpns = dbQuery.GetSelectedKeys(context.Selection.Keys)
                    .Select(l => new Lpn()
                    {
                        Tipo = l[3],
                        IdExterno = l[4]
                    })
                    .ToList();
            }

            uow.PedidoRepository.DesasociarLpn(pedido, lpns, uow.GetTransactionNumber());
        }
    }
}
