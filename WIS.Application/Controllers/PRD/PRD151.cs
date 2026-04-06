using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Produccion.DTOs;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD151 : AppController
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

        public PRD151(ISessionAccessor session,
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
        }

        public override PageContext PageLoad(PageContext context)
        {
            string nroIngreso = context.GetParameter("nuPrdcIngreso");

            if (string.IsNullOrEmpty(nroIngreso))
            {
                context.Redirect = "/PRD/PRD170";
                return context;
            }

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                DTOIngreso ingreso = uow.ProduccionRepository.GetIngresoPRD150(nroIngreso);
                context.AddParameter("ingreso", JsonConvert.SerializeObject(ingreso));
            }

            return context;
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
            switch (grid.Id)
            {
                case "PRD151_grid_1":
                    return this.Grid1FetchRows(grid, context);
                case "PRD151_grid_2":
                    return this.Grid2FetchRows(grid, context);
                case "PRD151_grid_3":
                    return this.Grid3FetchRows(grid, context);
                case "PRD151_grid_4":
                    return this.Grid4FetchRows(grid, context);
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
            switch (grid.Id)
            {
                case "PRD151_grid_1":
                    return this.Grid1FetchStats(grid, query);
                case "PRD151_grid_2":
                    return this.Grid2FetchStats(grid, query);
                case "PRD151_grid_3":
                    return this.Grid3FetchStats(grid, query);
                case "PRD151_grid_4":
                    return this.Grid4FetchStats(grid, query);
            }

            return null;
        }

        #region Metodos Auxiliares

        public virtual Grid Grid1FetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this.GridKeys = new List<string>
                {
                    "CD_COMPONENTE", "NU_PRIORIDAD", "CD_PRDC_DEFINICION"
                };

                string nuPrdcIngreso = context.GetParameter("nuPrdcIngreso");
                string cdFormula = context.GetParameter("cdPrdcDefinicion");

                var dbQuery = new SegumientoProduccionDetEntradaQuery(nuPrdcIngreso, cdFormula);

                uow.HandleQuery(dbQuery);

                List<SortCommand> defaultSorts = new List<SortCommand>();

                defaultSorts.Add(new SortCommand("DS_PRODUTO", SortDirection.Descending));

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSorts, this.GridKeys);
            }

            return grid;
        }

        public virtual Grid Grid2FetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this.GridKeys = new List<string>
                {
                    "CD_PRDC_DEFINICION", "CD_PRODUTO"
                };

                string cdFormula = context.GetParameter("cdPrdcDefinicion");
                string nuPrdcIngreso = context.GetParameter("nuPrdcIngreso");

                var dbQuery = new SegumientoProduccionDetSalidaQuery(nuPrdcIngreso, cdFormula);

                uow.HandleQuery(dbQuery);

                List<SortCommand> defaultSorts = new List<SortCommand>();

                defaultSorts.Add(new SortCommand("DS_PRODUTO", SortDirection.Descending));

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSorts, this.GridKeys);
            }

            return grid;
        }

        public virtual Grid Grid3FetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this.GridKeys = new List<string>
                {
                    "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_EMPRESA", "CD_FAIXA"
                };

                string nuPrdcIngreso = context.GetParameter("nuPrdcIngreso");
                var dbQuery = new SegumientoProduccionProducidoBBQuery(nuPrdcIngreso);

                uow.HandleQuery(dbQuery);

                List<SortCommand> defaultSorts = new List<SortCommand>();

                defaultSorts.Add(new SortCommand("DS_PRODUTO", SortDirection.Descending));

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSorts, this.GridKeys);
            }

            return grid;
        }

        public virtual Grid Grid4FetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this.GridKeys = new List<string>
                {
                    "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_EMPRESA", "CD_FAIXA"
                };

                string nuPrdcIngreso = context.GetParameter("nuPrdcIngreso");
                var dbQuery = new SegumientoProduccionConsumidoBBQuery(nuPrdcIngreso);

                uow.HandleQuery(dbQuery);

                List<SortCommand> defaultSorts = new List<SortCommand>();

                defaultSorts.Add(new SortCommand("DS_PRODUTO", SortDirection.Descending));

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSorts, this.GridKeys);
            }

            return grid;
        }

        public virtual GridStats Grid1FetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.GridKeys = new List<string>
            {
                "CD_COMPONENTE", "NU_PRIORIDAD", "CD_PRDC_DEFINICION"
            };

            string nuPrdcIngreso = query.GetParameter("nuPrdcIngreso");
            string cdFormula = query.GetParameter("cdPrdcDefinicion");

            var dbQuery = new SegumientoProduccionDetEntradaQuery(nuPrdcIngreso, cdFormula);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual GridStats Grid2FetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.GridKeys = new List<string>
            {
                "CD_PRDC_DEFINICION", "CD_PRODUTO"
            };

            string cdFormula = query.GetParameter("cdPrdcDefinicion");
            string nuPrdcIngreso = query.GetParameter("nuPrdcIngreso");

            var dbQuery = new SegumientoProduccionDetSalidaQuery(nuPrdcIngreso, cdFormula);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual GridStats Grid3FetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.GridKeys = new List<string>
            {
                "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_EMPRESA", "CD_FAIXA"
            };

            string nuPrdcIngreso = query.GetParameter("nuPrdcIngreso");
            var dbQuery = new SegumientoProduccionProducidoBBQuery(nuPrdcIngreso);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        
        public virtual GridStats Grid4FetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.GridKeys = new List<string>
            {
                "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_EMPRESA", "CD_FAIXA"
            };

            string nuPrdcIngreso = query.GetParameter("nuPrdcIngreso");
            var dbQuery = new SegumientoProduccionConsumidoBBQuery(nuPrdcIngreso);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #endregion
    }
}
