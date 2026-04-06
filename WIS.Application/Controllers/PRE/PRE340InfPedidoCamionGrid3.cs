using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE340InfPedidoCamionGrid3 : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys3 { get; }

        protected List<SortCommand> DefaultSort { get; }
        public PRE340InfPedidoCamionGrid3(
          ISecurityService security,
          IUnitOfWorkFactory uowFactory,
          IIdentityService identity,
          IFormValidationService formValidationService,
          IGridService gridService,
          IGridExcelService excelService,
          IFilterInterpreter filterInterpreter)
        {

            this.GridKeys3 = new List<string>
            {
               "CD_EMPRESA", "CD_CLIENTE", "NU_PEDIDO","CD_PRODUTO","NU_IDENTIFICADOR"
            };
            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Ascending),
            };

            _security = security;
            _uowFactory = uowFactory;
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _formValidationService = formValidationService;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        #region Grid

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        protected virtual PedidoDetalleSinCamionPre340QueryData GetPedidoDetalleSinCamionPre340QueryData(ComponentContext context)
        {
            string pedido = context.GetParameter("NU_PEDIDO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));
            string cliente = context.GetParameter("CD_CLIENTE");

            return new PedidoDetalleSinCamionPre340QueryData
            {
                Pedido = pedido,
                Cliente = cliente,
                Empresa = empresa,
            };
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var queryData = GetPedidoDetalleSinCamionPre340QueryData(context);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PedidoDetalleSinCamionPre340Query(queryData.Pedido, queryData.Cliente, queryData.Empresa);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys3);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var queryData = GetPedidoDetalleSinCamionPre340QueryData(context);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PedidoDetalleSinCamionPre340Query(queryData.Pedido, queryData.Cliente, queryData.Empresa);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var queryData = GetPedidoDetalleSinCamionPre340QueryData(context);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PedidoDetalleSinCamionPre340Query(queryData.Pedido, queryData.Cliente, queryData.Empresa);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #endregion
    }

    public class PedidoDetalleSinCamionPre340QueryData
    {
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
    }
}


