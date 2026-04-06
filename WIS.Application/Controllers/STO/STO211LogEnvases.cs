using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.STO
{
    public class STO211LogEnvases : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public STO211LogEnvases(IUnitOfWorkFactory uowFactory, IIdentityService identity, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_LOG_STOCK_ENVASE"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_LOG_STOCK_ENVASE", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            STO211Query dbQuery;

            if (query.Parameters.Count > 0)
            {
                string idEnvase = query.Parameters.FirstOrDefault(x => x.Id == "envase").Value;
                string ndTpEnvase = query.Parameters.FirstOrDefault(x => x.Id == "tipo").Value;

                dbQuery = new STO211Query(idEnvase, ndTpEnvase);

            }
            else
            {
                dbQuery = new STO211Query();
            }
            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            STO211Query dbQuery;
            if (query.Parameters.Count > 0)
            {
                string idEnvase = query.Parameters.FirstOrDefault(x => x.Id == "envase").Value;
                string ndTpEnvase = query.Parameters.FirstOrDefault(x => x.Id == "tipo").Value;

                dbQuery = new STO211Query(idEnvase, ndTpEnvase);

            }
            else
            {
                dbQuery = new STO211Query();
            }

            uow.HandleQuery(dbQuery, true);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_LOG_STOCK_ENVASE", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            STO211Query dbQuery;
            if (context.Parameters.Count > 0)
            {
                string idEnvase = context.Parameters.FirstOrDefault(x => x.Id == "envase").Value;
                string ndTpEnvase = context.Parameters.FirstOrDefault(x => x.Id == "tipo").Value;

                dbQuery = new STO211Query(idEnvase, ndTpEnvase);


            }
            else
            {
                dbQuery = new STO211Query();
            }

            uow.HandleQuery(dbQuery, true);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }
    }
}
