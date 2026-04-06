using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Produccion.Interfaces;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD110DetallesProducccion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly ISecurityService _securityService;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys1 { get; }
        protected List<SortCommand> DefaultSort1 { get; }

        protected List<string> GridKeys2 { get; }
        protected List<SortCommand> DefaultSort2 { get; }

        public PRD110DetallesProducccion(
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            ITrafficOfficerService concurrencyControl,
            ILogicaProduccionFactory logicaProduccion,
            IGridService gridService,
            ISecurityService securityService,
            IIdentityService identity,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._securityService = securityService;
            this._logicaProduccionFactory = logicaProduccion;
            this._gridValidationService = gridValidationService;
            this._identity = identity;
            this._concurrencyControl = concurrencyControl;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys1 = new List<string> { "NU_PRDC_INGRESO_REAL" };
            this.DefaultSort1 = new List<SortCommand> { new SortCommand("NU_PRDC_INGRESO_REAL", SortDirection.Ascending) };

            this.GridKeys2 = new List<string> { "NU_PRDC_SALIDA_REAL" };
            this.DefaultSort2 = new List<SortCommand> { new SortCommand("NU_PRDC_SALIDA_REAL", SortDirection.Ascending) };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var idIngreso = context.GetParameter("idIngreso");

            if (grid.Id == "PRD110DetallesProducccion_grid_1")
            {
                var dbQuery = new DetallesIngresoProduccionQuery(idIngreso);

                uow.HandleQuery(dbQuery);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort1, this.GridKeys1);

            }
            else if (grid.Id == "PRD110DetallesProducccion_grid_2")
            {
                var dbQuery = new DetallesSalidaProduccionQuery(idIngreso);

                uow.HandleQuery(dbQuery);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort2, this.GridKeys2);
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var idIngreso = context.GetParameter("idIngreso");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (grid.Id == "PRD110DetallesProducccion_grid_1")
                {
                    var dbQuery = new DetallesIngresoProduccionQuery(idIngreso);

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("NU_PRDC_INGRESO_REAL", SortDirection.Ascending);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }

                if (grid.Id == "PRD110DetallesProducccion_grid_2")
                {
                    var dbQuery = new DetallesSalidaProduccionQuery(idIngreso);

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("NU_PRDC_SALIDA_REAL", SortDirection.Ascending);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }

                return null;
            }
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var idIngreso = context.GetParameter("idIngreso");

            if (grid.Id == "PRD110DetallesProducccion_grid_1")
            {
                var dbQuery = new DetallesIngresoProduccionQuery(idIngreso);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "PRD110DetallesProducccion_grid_2")
            {
                var dbQuery = new DetallesSalidaProduccionQuery(idIngreso);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            string nuIngresoProduccion = context.GetParameter("idIngreso");
            using var uow = _uowFactory.GetUnitOfWork();
            ILogicaProduccion logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

            var ingreso = logicaProduccion.GetIngresoProduccion();

            form.GetField("idInternoProduccion").Value = ingreso.Id;
            form.GetField("idExternoProduccion").Value = ingreso.IdProduccionExterno;
            form.GetField("descripcionProduccion").Value = ingreso.Anexo1;

            return form;
        }
    }
}
