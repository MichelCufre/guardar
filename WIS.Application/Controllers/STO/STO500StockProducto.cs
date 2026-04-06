using System;
using System.Collections.Generic;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
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

namespace WIS.Application.Controllers.STO
{
    public class STO500StockProducto : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public STO500StockProducto(IUnitOfWorkFactory uowFactory, IIdentityService identity, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_PRODUTO","CD_EMPRESA", "NU_PREDIO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Ascending),
                new SortCommand("CD_PRODUTO", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnDetalle", "STO500_grid1_btn_Detalle", "fas fa-list")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new StockPorProductoQuery();

            uow.HandleQuery(dbQuery, predioNullMapValue: GeneralDb.SeparadorGuion);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new StockPorProductoQuery();

            uow.HandleQuery(dbQuery, predioNullMapValue: GeneralDb.SeparadorGuion);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new StockPorProductoQuery();

            uow.HandleQuery(dbQuery, predioNullMapValue: GeneralDb.SeparadorGuion);

            context.FileName = $"{this._identity.Application}_{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnDetalle")
            {
                context.Redirect("/stock/STO150", new List<ComponentParameter>() {
                      new ComponentParameter(){ Id = "empresa", Value = context.Row.GetCell("CD_EMPRESA").Value},
                      new ComponentParameter(){ Id = "producto", Value = context.Row.GetCell("CD_PRODUTO").Value},
                      new ComponentParameter(){ Id = "excluirAreaEquipoTransferencia", Value = "true"},
                });
            }

            return context;
        }
    }
}