using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Evento;
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

namespace WIS.Application.Controllers.EVT
{
    public class EVT050VerAdjuntos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EVT050VerAdjuntos(
            IIdentityService identity, 
            IUnitOfWorkFactory uowFactory, 
            IGridService gridService, 
            IGridExcelService excelService, 
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_EVENTO_NOTIFICACION_ARCHIVO",
                "NU_EVENTO_NOTIFICACION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_EVENTO_NOTIFICACION_ARCHIVO", SortDirection.Descending),
                new SortCommand("NU_EVENTO_NOTIFICACION", SortDirection.Descending),
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem>
            {
                new GridButton("btnDescargar", "EVT050VerAdjuntos_Sec0_btn_Descargar", "fas fa-file-download"),
            }));
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuEventoNotificacion = int.Parse(context.GetParameter("notificacion"));
            var dbQuery = new NotificacionesArchivosQuery(nuEventoNotificacion);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuEventoNotificacion = int.Parse(context.GetParameter("notificacion"));
            var dbQuery = new NotificacionesArchivosQuery(nuEventoNotificacion);

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

            var nuEventoNotificacion = int.Parse(context.GetParameter("notificacion"));
            var dbQuery = new NotificacionesArchivosQuery(nuEventoNotificacion);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
    }
}
