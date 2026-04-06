using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC003CalculosNoRealizadosError : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC003CalculosNoRealizadosError> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC003CalculosNoRealizadosError(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC003CalculosNoRealizadosError> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_EJECUCION",
                "NU_LINEA",
                "CD_FACTURACION",
                "NU_COMPONENTE",
                "CD_EMPRESA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_EJECUCION", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            CalculosNoRealizadosErrorQuery dbQuery;
            if (!string.IsNullOrEmpty(nuEjecucion))
                dbQuery = new CalculosNoRealizadosErrorQuery(int.Parse(nuEjecucion));
            else
                dbQuery = new CalculosNoRealizadosErrorQuery();

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            CalculosNoRealizadosErrorQuery dbQuery;
            if (!string.IsNullOrEmpty(nuEjecucion))
                dbQuery = new CalculosNoRealizadosErrorQuery(int.Parse(nuEjecucion));
            else
                dbQuery = new CalculosNoRealizadosErrorQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            CalculosNoRealizadosErrorQuery dbQuery;
            if (!string.IsNullOrEmpty(nuEjecucion))
                dbQuery = new CalculosNoRealizadosErrorQuery(int.Parse(nuEjecucion));
            else
                dbQuery = new CalculosNoRealizadosErrorQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
