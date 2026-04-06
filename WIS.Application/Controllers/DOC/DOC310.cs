using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DOC310 : AppController
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

        public DOC310(ISessionAccessor session,
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
                "DOCUMENTO_ACTA", "TIPO_DOCUMENTO_ACTA","DOCUMENTO_AFECTADO", "NU_DOCUMENTO_AFECTADO","TIPO_DOCUMENTO_AFECTADO"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                new GridItemHeader("DOC080_Sec0_lbl_Acciones"),
                new GridButton("btnDetalleActa", "DOC310_grid1_btn_DetalleActa", "fas fa-list"),
                new GridButton("btnDetalleDocAfectado", "DOC310_grid1_btn_DetalleDoc", "fas fa-list")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new DocumentosAsociadosActasQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("FECHA_AGREGADO_ACTA", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            var nuDocumento = string.Empty;
            var tpDocumento = string.Empty;

            switch (context.ButtonId)
            {
                case "btnDetalleActa":
                    nuDocumento = context.Row.Cells.FirstOrDefault(c => c.Column.Id == "DOCUMENTO_ACTA").Value;
                    tpDocumento = context.Row.Cells.FirstOrDefault(c => c.Column.Id == "TIPO_DOCUMENTO_ACTA").Value;
                    break;
                case "btnDetalleDocAfectado":
                    nuDocumento = context.Row.Cells.FirstOrDefault(c => c.Column.Id == "NU_DOCUMENTO_AFECTADO").Value;
                    tpDocumento = context.Row.Cells.FirstOrDefault(c => c.Column.Id == "TIPO_DOCUMENTO_AFECTADO").Value;
                    break;
            }

            context.Redirect("/documento/DOC096", new List<ComponentParameter>()
            {
                new ComponentParameter(){ Id = "nuDocumento", Value = nuDocumento },
                new ComponentParameter(){ Id = "tpDocumento", Value = tpDocumento },
            });

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new DocumentosAsociadosActasQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("FECHA_AGREGADO_ACTA", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new DocumentosAsociadosActasQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}