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
    public class PRE060Contenedores : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE060Contenedores(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREPARACION", "NU_CONTENEDOR"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREPARACION", SortDirection.Descending),
                new SortCommand("NU_CONTENEDOR", SortDirection.Descending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> 
            {
                new GridButton("btnDetalle", "PRE060_grid1_btn_Detalle", "fas fa-list"),
                new GridButton("btnImprimir", "PRE060_grid1_btn_Imprimir", "fas fa-print"),
                new GridButton("btnUTs", "PRE060_Sec0_btn_btnUTs", "fas fa-list")
            }));

            context.AddLink("CD_ENDERECO", "registro/REG040", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_ENDERECO", "ubicacion") });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ConsultaGeneralContenedoresQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                var manejaUT = !string.IsNullOrEmpty(row.GetCell("NU_UNIDAD_TRANSPORTE")?.Value);

                if (!manejaUT)
                    row.DisabledButtons.Add("btnUTs");
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ConsultaGeneralContenedoresQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ConsultaGeneralContenedoresQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnDetalle")
            {
                context.Redirect("/preparacion/PRE061", true, new List<ComponentParameter>()
                {
                    new ComponentParameter(){ Id = "contenedor", Value = context.Row.GetCell("NU_CONTENEDOR").Value},
                    new ComponentParameter(){ Id = "preparacion", Value = context.Row.GetCell("NU_PREPARACION").Value},
                });
            }
            else if (context.ButtonId == "btnUTs")
            {
                context.Redirect("/stock/STO750", true, new List<ComponentParameter>() 
                {
                    new ComponentParameter(){ Id = "nroUT", Value = context.Row.GetCell("NU_UNIDAD_TRANSPORTE").Value}
                });
            }

            return context;
        }
    }
}
