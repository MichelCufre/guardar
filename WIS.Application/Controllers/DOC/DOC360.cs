using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.DataModel.Queries.DocumentoVistaQuery;
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
    public class DOC360 : AppController
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

        public DOC360(ISessionAccessor session,
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
               "CD_EMPRESA"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem> {
                new GridButton("btnAjusteNegativo", "DOC360_Sec0_btn_AjusteNegativo", "fas fa-minus-square"),
                new GridButton("btnAjustePositivo", "DOC360_Sec0_btn_AjustePositivo", "fas fa-plus-square")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var query = new AjusteStockVistaQuery();

                uow.HandleQuery(query);

                var defaultSort = new SortCommand("CD_EMPRESA", SortDirection.Descending);
                grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);
            }
            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new DocumentoVistaQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("CD_EMPRESA", SortDirection.Descending);

                context.FileName = "DocumentoAjusteStock_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            var ajustePositivo = false;

            switch (context.ButtonId)
            {
                case "btnAjustePositivo":
                    ajustePositivo = true;
                    break;
            }

            context.Redirect("/documento/DOC361", new List<ComponentParameter>()
            {
                new ComponentParameter(){ Id = "cdEmpresa", Value = context.Row.Cells.FirstOrDefault(c => c.Column.Id == "CD_EMPRESA").Value },
                new ComponentParameter(){ Id = "ajPositivo", Value = ajustePositivo.ToString() },
            });

            return context;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var query = new AjusteStockVistaQuery();

            uow.HandleQuery(query);
            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = query.GetCount()
            };
        }
    }
}
