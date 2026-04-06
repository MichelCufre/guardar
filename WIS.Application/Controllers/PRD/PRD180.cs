using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD180 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; set; }
        
        public PRD180(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "NU_PRDC_INGRESO, QT_PASADAS, NU_ORDEN"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this.GridKeys = new List<string>
                {
                    "NU_PRDC_INGRESO", "QT_PASADAS", "NU_ORDEN"
                };

                string nuPrdcIngreso = context.GetParameter("nuPrdcIngreso");
                var dbQuery = new PasadasDeIngresoProduccionQuery(nuPrdcIngreso);

                uow.HandleQuery(dbQuery);

                List<SortCommand> defaultSorts = new List<SortCommand>();

                defaultSorts.Add(new SortCommand("NU_PRDC_INGRESO", SortDirection.Descending));
                defaultSorts.Add(new SortCommand("QT_PASADAS", SortDirection.Descending));
                defaultSorts.Add(new SortCommand("NU_ORDEN", SortDirection.Descending));

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSorts, this.GridKeys);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string nuPrdcIngreso = context.GetParameter("nuPrdcIngreso");
                var dbQuery = new PasadasDeIngresoProduccionQuery(nuPrdcIngreso);

                uow.HandleQuery(dbQuery);

                List<SortCommand> defaultSorts = new List<SortCommand>();

                defaultSorts.Add(new SortCommand("NU_PRDC_INGRESO", SortDirection.Descending));
                defaultSorts.Add(new SortCommand("QT_PASADAS", SortDirection.Descending));
                defaultSorts.Add(new SortCommand("NU_ORDEN", SortDirection.Descending));

                context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSorts);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.GridKeys = new List<string>
                {
                    "NU_PRDC_INGRESO", "QT_PASADAS", "NU_ORDEN"
                };

            string nuPrdcIngreso = query.GetParameter("nuPrdcIngreso");
            var dbQuery = new PasadasDeIngresoProduccionQuery(nuPrdcIngreso);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }        
    }
}
