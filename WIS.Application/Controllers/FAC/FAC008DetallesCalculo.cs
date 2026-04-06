using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Exceptions;
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
    public class FAC008DetallesCalculo : AppController
    {
        protected readonly IGridService _gridService;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC008DetallesCalculo> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC008DetallesCalculo(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC008DetallesCalculo> logger,
            IGridValidationService gridValidationService)
        {

            this.GridKeys = new List<string>
            {
                "NU_RESULTADO_DETALLE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_GENERICO",SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }


        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "nuEjecucion")?.Value, out int nuEjecucion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "cdEmpresa")?.Value, out int emp))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string cdFacturacion = context.Parameters.FirstOrDefault(s => s.Id == "cdFacturacion")?.Value;
            if (string.IsNullOrEmpty(cdFacturacion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string nuComponente = context.Parameters.FirstOrDefault(s => s.Id == "nuComponente")?.Value;
            if (string.IsNullOrEmpty(nuComponente))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var query = new FAC008Query(nuEjecucion, emp, cdFacturacion, nuComponente);
            uow.HandleQuery(query);
            grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "nuEjecucion")?.Value, out int nuEjecucion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "cdEmpresa")?.Value, out int emp))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string cdFacturacion = context.Parameters.FirstOrDefault(s => s.Id == "cdFacturacion")?.Value;
            if (string.IsNullOrEmpty(cdFacturacion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string nuComponente = context.Parameters.FirstOrDefault(s => s.Id == "nuComponente")?.Value;
            if (string.IsNullOrEmpty(nuComponente))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new FAC008Query(nuEjecucion, emp, cdFacturacion, nuComponente);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(query.Parameters.FirstOrDefault(x => x.Id == "nuEjecucion")?.Value, out int nuEjecucion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "cdEmpresa")?.Value, out int emp))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string cdFacturacion = query.Parameters.FirstOrDefault(s => s.Id == "cdFacturacion")?.Value;
            if (string.IsNullOrEmpty(cdFacturacion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string nuComponente = query.Parameters.FirstOrDefault(s => s.Id == "nuComponente")?.Value;
            if (string.IsNullOrEmpty(nuComponente))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new FAC008Query(nuEjecucion, emp, cdFacturacion, nuComponente);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);
            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

    }
}
