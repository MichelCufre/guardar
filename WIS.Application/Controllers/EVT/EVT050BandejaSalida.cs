using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Eventos;
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
    public class EVT050BandejaSalida : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EVT050BandejaSalida(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_EVENTO_NOTIFICACION",
            };

            this.DefaultSort = new List<SortCommand>
            {
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
                new GridButton("btnVerMensaje", "EVT050_Sec0_btn_VerMensaje", "fas fa-folder-open"),
                new GridButton("btnVerAdjuntos", "EVT050_Sec0_btn_VerAdjuntos", "fas fa-paperclip"),
                new GridButton("btnReenviar", "EVT050_Sec0_btn_Reenviar", "fas fa-undo-alt")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            NotificacionesEventosQuery dbQuery = null;

            dbQuery = new NotificacionesEventosQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            DisableButtons(grid.Rows, uow);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new NotificacionesEventosQuery();

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

            NotificacionesEventosQuery dbQuery = null;

            dbQuery = new NotificacionesEventosQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            try
            {
                if (context.ButtonId == "btnReenviar")
                {
                    using var uow = this._uowFactory.GetUnitOfWork();

                    var numeroNotificacion = long.Parse(context.GetParameter("numeroNotificacion"));
                    var notificacion = uow.NotificacionRepository.GetNotificacionEmail(numeroNotificacion);

                    notificacion.Estado = EstadoNotificacion.EST_PEND;
                    notificacion.FechaRenvio = DateTime.Now;

                    uow.NotificacionRepository.UpdateNotificacionEmail(notificacion);

                    uow.SaveChanges();
                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }

            return context;
        }

        #region MEtodos Auxiliares

        public virtual void DisableButtons(List<GridRow> rows, IUnitOfWork uow)
        {
            foreach (var row in rows)
            {
                var tipoNotificacion = row.GetCell("TP_NOTIFICACION").Value;
                var estado = row.GetCell("ND_ESTADO").Value;

                var estadoEnum = EstadoBandejaHelper.GetEstado(estado);
                var tipoNotificacionEnum = TipoNotificacionHelper.GetTipoNotificacion(tipoNotificacion);

                if (estadoEnum == EstadoBandeja.EST_PEND)
                    row.DisabledButtons.Add("btnReenviar");
            }
        }

        #endregion
    }
}
