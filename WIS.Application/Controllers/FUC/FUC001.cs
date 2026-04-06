using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries;
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

namespace WIS.Application.Controllers.FUC
{
    public class FUC001 : AppController
    {
        protected readonly IGridService _gridService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IIdentityService _identity;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FUC001(IGridService gridService,
            IUnitOfWorkFactory uowFactory,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            IIdentityService identity)
        {
            GridKeys = new List<string>
            {
                "CD_ARCHIVO",
            };

            DefaultSort = new List<SortCommand>
            {
                new SortCommand("NM_ARCHIVO", SortDirection.Ascending),
            };

            _gridService = gridService;
            _uowFactory = uowFactory;
            _gridExcelService = gridExcelService;
            _filterInterpreter = filterInterpreter;
            _identity = identity;
        }

        #region >> Grid


        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                new GridItemHeader("FUC001_Sec0_lbl_Acciones"),
                new GridButton("btnDescargar", "General_Sec0_btn_DescargarDocumento", "fas fa-list"),
                new GridButton("btnBorrar", "General_Sec0_btn_BorrarDocumentos", "fas fa-clipboard-list"),
            }));

            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var tpEntidad = context.GetParameter("tipoEntidad");
            var cdEntidad = context.GetParameter("codigoEntidad");
            var permiteBaja = context.GetParameter("permiteBaja");

            var dbQuery = new FileQuery(tpEntidad, cdEntidad);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, GridKeys);

            grid.Rows.ForEach(row =>
            {
                if (!bool.Parse(permiteBaja))
                    row.DisabledButtons.Add("btnBorrar");
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var tpEntidad = context.GetParameter("tipoEntidad");
            var cdEntidad = context.GetParameter("codigoEntidad");

            var dbQuery = new FileQuery(tpEntidad, cdEntidad);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(_filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var tpEntidad = context.GetParameter("tipoEntidad");
            var cdEntidad = context.GetParameter("codigoEntidad");

            var dbQuery = new FileQuery(tpEntidad, cdEntidad);

            uow.HandleQuery(dbQuery);

            context.FileName = _identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return _gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnBorrar")
            {
                context.Row.IsDeleted = true;
            }

            return context;
        }

        #endregion << Grid
    }
}
