using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.DOC
{
    public class DOC035 : AppController
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

        protected List<string> GridKeys { get; }

        public DOC035(ISessionAccessor session,
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
                "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "CD_FAIXA", "TP_DOCUMENTO", "NU_DOCUMENTO"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnDetalles", "General_Sec0_btn_Detalles", "fas fa-arrow-right")
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var query = new DetallesIngresoQuery();

                uow.HandleQuery(query);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            var rows = new List<ComponentParameter>();

            foreach (var r in context.Row.Cells)
            {
                rows.Add(new ComponentParameter
                {
                    Id = r.Column.Id,
                    Value = r.Value
                });
            }
            _session.SetValue("DOC082_ROWS", JsonConvert.SerializeObject(rows));

            context.Redirect("/documento/DOC082", new List<ComponentParameter>() { });

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new DetallesIngresoQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = "DetallesIngreso_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            DetallesIngresoQuery query = new DetallesIngresoQuery();

            uow.HandleQuery(query);
            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = query.GetCount()
            };
        }
    }
}
