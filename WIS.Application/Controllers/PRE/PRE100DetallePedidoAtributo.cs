using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100DetallePedidoAtributo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IParameterService _paramService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE100DetallePedidoAtributo(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IParameterService paramService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_FAIXA", "ID_ESPECIFICA_IDENTIFICADOR", "NU_DET_PED_SAI_ATRIB"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._paramService = paramService;
        }

        public override PageContext PageLoad(PageContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cdEmpresa = int.Parse(context.GetParameter("empresa"));
            var cdCliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            var pedido = uow.PedidoRepository.GetPedido(cdEmpresa, cdCliente, nuPedido);
            var atributosHabilitados = (_paramService.GetValueByEmpresa(ParamManager.IE_503_HAB_ATRIBUTOS, cdEmpresa) ?? "N") == "S";

            context.Parameters.Add(new ComponentParameter("editable", (pedido.IsManual && atributosHabilitados) ? "S" : "N"));

            var empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);
            var cliente = uow.AgenteRepository.GetAgente(cdEmpresa, cdCliente);

            context.AddParameter("empresaNombre", empresa.Nombre);
            context.AddParameter("agenteDescripcion", cliente.Descripcion);
            context.AddParameter("agenteCodigo", cliente.Codigo);
            context.AddParameter("agenteTipo", cliente.Tipo);

            return context;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsCommitEnabled = false;
            context.IsEditingEnabled = false;

            var empresaId = int.Parse(context.GetParameter("empresa"));
            var clienteId = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

			var pedido = uow.PedidoRepository.GetPedido(empresaId, clienteId, nuPedido);
            var atributosHabilitados = (_paramService.GetValueByEmpresa(ParamManager.IE_503_HAB_ATRIBUTOS, empresaId) ?? "N") == "S";

			if (pedido.IsManual && atributosHabilitados)
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
                {
                    new GridButton("btnEditar", "PRE100DetallePedidoAtributo_grid1_btn_Atributos", "fas fa-edit"),
                }));
            }
            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

            var dbQuery = new DetallePedidoAtributoQuery(pedido);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

            var dbQuery = new DetallePedidoAtributoQuery(pedido);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var empresa = int.Parse(context.GetParameter("empresa"));
            var cliente = context.GetParameter("cliente");
            var nuPedido = context.GetParameter("pedido");

            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

            var dbQuery = new DetallePedidoAtributoQuery(pedido);
            uow.HandleQuery(dbQuery);
            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
    }
}
