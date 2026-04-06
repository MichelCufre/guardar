using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100PanelPedidos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IParameterService _paramService;
        protected readonly ISecurityService _security;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE100PanelPedidos(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IParameterService paramService,
            ISecurityService security)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._paramService = paramService;
            this._security = security;

        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            form.GetField("ND_ACTIVIDAD").Value = "true";

            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem>
            {
                new GridButton("btnEditar", "PRE100_grid1_btn_Editar", "fas fa-edit"),
                new GridButton("btnDetalle", "PRE100_grid1_btn_Detalle", "fas fa-list"),
                new GridItemDivider(),
                new GridItemHeader("LPN"),
                new GridButton("btnPedidoLpn", "PRE100_grid1_btn_PedidoLpn", "fas fa-list"),
                new GridButton("btnDetallesPedidoLpn", "PRE100_grid1_btn_DetallesPedidoLpn", "fas fa-list"),
                new GridButton("btnDetallesPedidoAtributo", "PRE100_grid1_btn_DetallesPedidoAtributo", "fas fa-list"),
                new GridItemDivider(),
                new GridButton("btnDetallesPedido", "PRE100_grid1_btn_detallesPedido", "fas fa-box-open"),
            }));

            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PedidosPanelQuery query;

            if (context.Parameters.Count > 0)
            {
                var pedidosActivos = bool.Parse(context.Parameters.FirstOrDefault(s => s.Id == "pedidoActivos").Value);

                query = new PedidosPanelQuery(pedidosActivos);
            }
            else
                query = new PedidosPanelQuery();

            uow.HandleQuery(query);
            grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

            DisableButtons(grid, uow);
            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PedidosPanelQuery query;

            if (context.Parameters.Count > 0)
            {
                var pedidosActivos = bool.Parse(context.Parameters.FirstOrDefault(s => s.Id == "pedidoActivos").Value);

                query = new PedidosPanelQuery(pedidosActivos);
            }
            else
                query = new PedidosPanelQuery();

            uow.HandleQuery(query);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, query, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PedidosPanelQuery query;

            if (context.Parameters.Count > 0)
            {
                var pedidosActivos = bool.Parse(context.Parameters.FirstOrDefault(s => s.Id == "pedidoActivos").Value);

                query = new PedidosPanelQuery(pedidosActivos);
            }
            else
                query = new PedidosPanelQuery();

            uow.HandleQuery(query);
            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats()
            {
                Count = query.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuPedido = context.Row.GetCell("NU_PEDIDO").Value;
            var cliente = context.Row.GetCell("CD_CLIENTE").Value;
            var empresa = int.Parse(context.Row.GetCell("CD_EMPRESA").Value);

            if (context.ButtonId == "btnDetallesPedido")
            {
                context.Redirect("/preparacion/PRE150", true, new List<ComponentParameter> {
                    new ComponentParameter("pedido", nuPedido),
                    new ComponentParameter("cliente", cliente),
                    new ComponentParameter("empresa", empresa.ToString())
                });
            }
            else
            {
                var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

                if (!pedido.PuedeModificarse())
                    throw new ValidationFailedException("General_Sec0_Error_Er092_SituacionNoPermiteEdicion");
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual void DisableButtons(Grid grid, IUnitOfWork uow)
        {
            Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.PRE100_grid1_btn_Editar,
                SecurityResources.PRE100_grid1_btn_Detalle,
                SecurityResources.PRE100_grid1_btn_DetallesPedido,
                SecurityResources.PRE100_grid1_btn_PedidoLpn,
                SecurityResources.PRE100_grid1_btn_DetallesPedidoLpn,
                SecurityResources.PRE100_grid1_btn_DetallesPedidoAtributo,
            });

            var empresas = uow.EmpresaRepository.GetEmpresasAsignadasUsuario(_identity.UserId);
            var lpnHabilitado = _paramService.GetValuesByEmpresa(ParamManager.IE_503_HAB_LPN, empresas);

            foreach (var row in grid.Rows)
            {
                var manual = row.GetCell("ID_MANUAL").Value == "S";
                var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

                if (!manual || lpnHabilitado[empresa] != "S" || !result[SecurityResources.PRE100_grid1_btn_PedidoLpn])
                    row.DisabledButtons.Add("btnPedidoLpn");

                if (!result[SecurityResources.PRE100_grid1_btn_Editar])
                    row.DisabledButtons.Add("btnEditar");

                if (!result[SecurityResources.PRE100_grid1_btn_Detalle])
                    row.DisabledButtons.Add("btnDetalle");

                if (!result[SecurityResources.PRE100_grid1_btn_DetallesPedido])
                    row.DisabledButtons.Add("btnDetallesPedido");

                if (!result[SecurityResources.PRE100_grid1_btn_DetallesPedidoLpn])
                    row.DisabledButtons.Add("btnDetallesPedidoLpn");

                if (!result[SecurityResources.PRE100_grid1_btn_DetallesPedidoAtributo])
                    row.DisabledButtons.Add("btnDetallesPedidoAtributo");
            }
        }

        #endregion
    }
}
