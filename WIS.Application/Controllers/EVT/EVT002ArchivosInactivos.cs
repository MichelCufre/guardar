using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Evento;
using WIS.Domain.Eventos;
using WIS.Domain.Eventos.Enums;
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
using WIS.Http;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.EVT
{
    public class EVT002ArchivosInactivos : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IWebApiClient _apiClient;
        protected readonly IParameterService _parameterService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public EVT002ArchivosInactivos(
            ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IWebApiClient apiClient,
            IParameterService parameterService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_ARCHIVO_ADJUNTO","CD_EMPRESA","CD_MANEJO","DS_REFERENCIA"
            };

            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._apiClient = apiClient;
            this._parameterService = parameterService;
            this._security = security;
            _filterInterpreter = filterInterpreter;

        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var listButton = new List<IGridItem>
            {
                //new GridButton("btnVer", "General_Sec0_btn_Ver", "far fa-eye"),
            };

            if (this._security.IsUserAllowed("EVT000_grid1_btn_Borrar"))
                listButton.Add(new GridButton("btnBorrar", "General_Sec0_btn_Borrar", ""));

            if (this._security.IsUserAllowed("General_Sec0_btn_Inactivar"))
                listButton.Add(new GridButton("btnActivar", "General_Sec0_btn_Activar", ""));

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", listButton));


            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_ARCHIVO_ADJUNTO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ArchivosInactivosQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_ARCHIVO_ADJUNTO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ArchivosInactivosQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ArchivosInactivosQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnDetalles")
            {
                _session.SetValue("EVT000_NU_ARCHIVO_ADJUNTO", context.Row.GetCell("NU_ARCHIVO_ADJUNTO").Value);
                context.Redirect("/evento/EVT001");
                return context;
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            long nuArchivoAdjunto = context.Row.GetCell("NU_ARCHIVO_ADJUNTO").Value.ToNumber<long>();

            if (context.ButtonId == "btnBorrar")
            {
                string ruta = this._parameterService.GetValue("PATH_ARCHIVOS_DIGITALES");
                List<string> listRutas = uow.ArchivoRepository.DeleteArchivo(nuArchivoAdjunto, ruta);
                uow.SaveChanges();

                this.BorrarArchivo(this._parameterService.GetValue("IP_ARCHIVOS_DIGITALES"), listRutas);

                context.AddSuccessNotification("General_Db_Success_Delete");

            }
            else if (context.ButtonId == "btnInactivar")
            {
                ArchivoAdjunto archivo = uow.ArchivoRepository.GetArchivoAdjunto(nuArchivoAdjunto);
                archivo.CD_SITUACAO = (short)EstadoArchivo.Inactivo;
                uow.ArchivoRepository.UpdateArchivoAdjunto(archivo, null);
                uow.SaveChanges();

                context.AddSuccessNotification("General_Db_Success_Inactive");

            }
            else if (context.ButtonId == "btnActivar")
            {
                ArchivoAdjunto archivo = uow.ArchivoRepository.GetArchivoAdjunto(nuArchivoAdjunto);
                archivo.CD_SITUACAO = (short)EstadoArchivo.Activo;
                uow.ArchivoRepository.UpdateArchivoAdjunto(archivo, null);
                uow.SaveChanges();

                context.AddSuccessNotification("General_Db_Success_Active");

            }
            else if (context.ButtonId == "btnVideo")
            {

                var splitRuta = context.Row.GetCell("LK_RUTA").Value.Split('\\').ToList();
                splitRuta = splitRuta.Take(splitRuta.Count() - 1).ToList();

                context.AddParameter("LK_RUTA", $"{this._parameterService.GetValue("COMPARTIDA_ARCHIVOS_DIGITALES")}{string.Join("\\", splitRuta)}");
            }

            return context;
        }

        #region Metodos Auxiliares

        protected virtual string BorrarArchivo(string url, List<string> listRutas)
        {
            var result = this._apiClient.Post(new Uri(new Uri(url), "BorrarArchivos"), JsonConvert.SerializeObject(listRutas));

            return result.Content.ReadAsStringAsync().Result;
        }

        #endregion
    }
}