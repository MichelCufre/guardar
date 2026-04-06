using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD240 : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public PRD240(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "NU_MOVIMIENTO_BB"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new MovimientosStockBlackBoxPRD240Query();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_MOVIMIENTO", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new MovimientosStockBlackBoxPRD240Query();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_MOVIMIENTO", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new MovimientosStockBlackBoxPRD240Query();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
