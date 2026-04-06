using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC012FacturacionPreviewPrecios : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC012FacturacionPreviewPrecios> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC012FacturacionPreviewPrecios(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC012FacturacionPreviewPrecios> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_EJECUCION",
                "CD_EMPRESA",
                "NU_COMPONENTE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_EJECUCION", SortDirection.Ascending),
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
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnDetalle", "FAC012_grid1_btn_Detalle", "fas fa-list"),
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? nroEject = null;
            if (int.TryParse(query.GetParameter("nuEjecucion"), out int parsedValue))
                nroEject = parsedValue;

            var dbQuery = new PreviewPreciosQuery(nroEject, SituacionDb.EJECUCION_PENDIENTE);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            return grid;
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnDetalle":
                    context.Redirect("/facturacion/FAC008", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                        new ComponentParameter(){ Id = "cdFacturacion", Value = context.Row.GetCell("CD_FACTURACION").Value },
                        new ComponentParameter(){ Id = "cdEmpresa", Value = context.Row.GetCell("CD_EMPRESA").Value },
                        new ComponentParameter(){ Id = "nuComponente", Value = context.Row.GetCell("NU_COMPONENTE").Value },
                    });
                    break;
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? nroEject = null;
            if (int.TryParse(query.GetParameter("nuEjecucion"), out int parsedValue))
                nroEject = parsedValue;

            var dbQuery = new PreviewPreciosQuery(nroEject, SituacionDb.EJECUCION_PENDIENTE);
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? nroEject = null;
            if (int.TryParse(query.GetParameter("nuEjecucion"), out int parsedValue))
                nroEject = parsedValue;

            var dbQuery = new PreviewPreciosQuery(nroEject, SituacionDb.EJECUCION_PENDIENTE);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
