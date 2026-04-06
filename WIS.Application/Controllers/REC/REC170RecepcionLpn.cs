using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC170RecepcionLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC170RecepcionLpn(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl)
        {
            this.GridKeys = new List<string>
            {
                "NU_AGENDA", "NU_LPN"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW",SortDirection.Descending)
            };

            _uowFactory = uowFactory;
            _concurrencyControl = concurrencyControl;
            _gridService = gridService;
            _filterInterpreter = filterInterpreter;
            _excelService = excelService;
            _identity = identity;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? nuAgenda = -1;

            if (!string.IsNullOrEmpty(context.GetParameter("keyAgenda")))
                nuAgenda = int.Parse(context.GetParameter("keyAgenda"));

            if (nuAgenda != -1)
            {
                RecepcionLpnQuery dbQuery = new RecepcionLpnQuery(nuAgenda);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            int? nuAgenda = -1;
            RecepcionLpnQuery dbQuery = new RecepcionLpnQuery();

            if (!string.IsNullOrEmpty(query.GetParameter("keyAgenda")))
                nuAgenda = int.Parse(query.GetParameter("keyAgenda"));

            if (nuAgenda != -1)
                dbQuery = new RecepcionLpnQuery(nuAgenda);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            int? nuAgenda = -1;
            RecepcionLpnQuery dbQuery = new RecepcionLpnQuery();

            if (!string.IsNullOrEmpty(context.GetParameter("keyAgenda")))
                nuAgenda = int.Parse(context.GetParameter("keyAgenda"));

            if (nuAgenda != -1)
                dbQuery = new RecepcionLpnQuery(nuAgenda);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
    }
}
