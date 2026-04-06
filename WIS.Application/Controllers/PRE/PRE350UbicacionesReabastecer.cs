using System;
using System.Collections.Generic;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
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

namespace WIS.Application.Controllers.PRE
{
    public class PRE350UbicacionesReabastecer : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE350UbicacionesReabastecer(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA", "CD_PRODUTO", "NU_IDENTIFICADOR_PI", "CD_ENDERECO_PI", "CD_FAIXA", "QT_PADRAO_PI"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Descending),
                new SortCommand("CD_PRODUTO", SortDirection.Descending)
            };
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem>
            {
                new GridButton("btnRedirect", "PRE350_grid1_btn_Redirect", "fas fa-boxes")
            }));


            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PRE350StockPickingReabastQuery dbQuery = new PRE350StockPickingReabastQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            foreach (var row in grid.Rows)
            {
                string urgente = row.GetCell("FL_NECESITA_URGENTE").Value;

                if (urgente.Equals("S"))
                {
                    row.CssClass = row.CssClass + " error";
                }
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PRE350StockPickingReabastQuery dbQuery = new PRE350StockPickingReabastQuery();

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

            PRE350StockPickingReabastQuery dbQuery = new PRE350StockPickingReabastQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            string empresa = context.Row.GetCell("CD_EMPRESA").Value;
            string producto = context.Row.GetCell("CD_PRODUTO").Value;
            string ubicacionPicking = context.Row.GetCell("CD_ENDERECO_PI").Value;
            string faixa = context.Row.GetCell("CD_FAIXA").Value;

            if (context.ButtonId == "btnRedirect")
            {
                context.Redirect("/preparacion/PRE351", new List<ComponentParameter>{
                    new ComponentParameter("empresa", empresa),
                    new ComponentParameter("producto", producto),
                    new ComponentParameter("ubicacionPicking", ubicacionPicking),
                    new ComponentParameter("faixa", faixa)
                });
            }
            return context;
        }
    }
}
