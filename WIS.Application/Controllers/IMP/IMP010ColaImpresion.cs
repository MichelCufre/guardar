using System;
using System.Collections.Generic;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Impresion;
using WIS.Domain.Services.Interfaces;
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

namespace WIS.Application.Controllers.IMP
{
    public class IMP010ColaImpresion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IPrintingService _printingService;
        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public IMP010ColaImpresion(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IPrintingService printingService)
        {
            this.GridKeys = new List<string>
            {
                "NU_IMPRESION",
            };

            this.Sorts = new List<SortCommand> {
                new SortCommand("NU_IMPRESION", SortDirection.Descending),
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._printingService = printingService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnDetalle", "LIMP010_grid1_btn_Detalle", "fas fa-list"),
                new GridButton("btnReenviar", "LIMP010_grid1_btn_Reenviar", "fas fa-undo-alt")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ColaImpresionQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ColaImpresionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ColaImpresionQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnDetalle")
                context.Redirect("/impresion/IMP011", new List<ComponentParameter>() { new ComponentParameter("impresion", context.Row.GetCell("NU_IMPRESION").Value) });
            else if (context.ButtonId == "btnReenviar")
                ReenviarImpresion(context);

            return context;
        }

        protected virtual void ReenviarImpresion(GridButtonActionContext context)
        {
            var nuImpresion = int.Parse(context.Row.Cells.Find(f => f.Column.Id == "NU_IMPRESION").Value);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                uow.BeginTransaction();
                uow.ImpresionRepository.RegenerarImpresion(nuImpresion, _printingService.GetEstadoInicial(), _identity.UserId);
                uow.SaveChanges();
                uow.Commit();
            }

            _printingService.SendToPrint(nuImpresion);
        }
    }
}
