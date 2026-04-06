using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Evento;
using WIS.Domain.Eventos;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.EVT
{
    public class EVT060PlantillasNotificaciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<EVT060CreatePlantilla> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EVT060PlantillasNotificaciones(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ILogger<EVT060CreatePlantilla> logger)
        {

            this.GridKeys = new List<string>
            {
                "NU_EVENTO",
                "TP_NOTIFICACION",
                "CD_LABEL_ESTILO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending),
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
        }

        public override PageContext PageLoad(PageContext data)
        {
            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_EDITAR", new List<GridButton>
            {
                new GridButton("btnEditar", "EXP040_grid1_btn_Editar", "fas fa-edit")
            }));

            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PlantillasNotificacionesQuery dbQuery = null;

            dbQuery = new PlantillasNotificacionesQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PlantillasNotificacionesQuery dbQuery = null;

            dbQuery = new PlantillasNotificacionesQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PlantillasNotificacionesQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.ButtonId == "btnEditar")
            {
                int nuEvento = Convert.ToInt32(context.Row.GetCell("NU_EVENTO").Value);
                string tpNotificacion = context.Row.GetCell("TP_NOTIFICACION").Value;
                string cdPlantilla = context.Row.GetCell("CD_LABEL_ESTILO").Value;

                EventoTemplate plantilla = uow.EventoRepository.GetEventoTemplate(nuEvento, tpNotificacion, cdPlantilla);
            }

            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("Eliminación de plantillas de notificaciones");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {

                        if (row.IsDeleted)
                        {
                            int nuEvento = int.TryParse(row.GetCell("NU_EVENTO").Value, out int evt) ? evt : 0;
                            string tpNotificacion = row.GetCell("TP_NOTIFICACION").Value;
                            string cdPlantilla = row.GetCell("CD_LABEL_ESTILO").Value;

                            if (uow.EventoRepository.AnyInstancia(nuEvento, tpNotificacion, cdPlantilla))
                            {
                                throw new EntityNotFoundException("EVT060_Sec0_Error_Er001_TemplateAsociadoInstancia");
                            }

                            var plantilla = uow.EventoRepository.GetEventoTemplate(nuEvento, tpNotificacion, cdPlantilla);

                            plantilla.FechaModificacion = DateTime.Now;
                            plantilla.NumeroTransaccion = nuTransaccion;
                            plantilla.NumeroTransaccionDelete = nuTransaccion;

                            uow.EventoRepository.UpdateTemplate(plantilla);
                            uow.SaveChanges();

                            uow.EventoRepository.RemoveTemplate(plantilla);
                            uow.SaveChanges();

                            uow.Commit();
                        }
                    }
                }

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (EntityNotFoundException ex)
            {
                this._logger.LogError(ex.Message, "EVT060GridCommit");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "EVT060GridCommit");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return grid;
        }
    }
}
