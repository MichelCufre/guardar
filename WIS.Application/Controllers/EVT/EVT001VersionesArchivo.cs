using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Evento;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
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

namespace WIS.Application.Controllers.EVT
{
    public class EVT001VersionesArchivo : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IParameterService _parameterService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public EVT001VersionesArchivo(
            ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IParameterService parameterService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_ARCHIVO_ADJUNTO","CD_EMPRESA","CD_MANEJO","DS_REFERENCIA","NU_VERSION"
            };

            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._parameterService = parameterService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;

        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            long? archivoAdjunto = _session.GetValue<string>("EVT000_NU_ARCHIVO_ADJUNTO").ToNumber<long?>();
            _session.SetValue("EVT000_NU_ARCHIVO_ADJUNTO", null);

            _session.SetValue("EVT001_NU_ARCHIVO_ADJUNTO", archivoAdjunto);

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnVer", "General_Sec0_btn_Ver", "far fa-eye"),
            }));

            using var uow = this._uowFactory.GetUnitOfWork();

            context.FetchContext.AddParameter("IP_COMPARTIDA", $"{this._parameterService.GetValue("IP_ARCHIVOS_DIGITALES")}/ARCHIVOS_DEGITALES/");

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_ARCHIVO_ADJUNTO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            VersionesArchivosQuery dbQuery = null;

            dbQuery = new VersionesArchivosQuery(_session.GetValue<long?>("EVT001_NU_ARCHIVO_ADJUNTO"));

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_ARCHIVO_ADJUNTO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            VersionesArchivosQuery dbQuery = null;

            dbQuery = new VersionesArchivosQuery(_session.GetValue<long?>("EVT001_NU_ARCHIVO_ADJUNTO"));

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new VersionesArchivosQuery(_session.GetValue<long?>("EVT001_NU_ARCHIVO_ADJUNTO"));

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
