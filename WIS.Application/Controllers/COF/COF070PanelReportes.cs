using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Configuracion;
using WIS.Domain.Services.Interfaces;
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
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.COF
{
    public class COF070PanelReportes : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITaskQueueService _taskQueue;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public COF070PanelReportes(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ITaskQueueService taskQueue)
        {
            this.GridKeys = new List<string>
            {
                "NU_REPORTE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_REPORTE", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._taskQueue = taskQueue;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnReimprimir", "COF070_grid1_btn_Reimprimir"),
                new GridButton("btnReprocesar", "COF070_grid1_btn_Reprocesar"),
                new GridButton("btnDescargar", "COF070_grid1_btn_Descargar")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConfiguracionReporteQuery dbQuery;

            if (!string.IsNullOrEmpty(context.GetParameter("camion")))
            {
                dbQuery = new ConfiguracionReporteQuery(context.GetParameter("camion"), string.Empty);
                grid.GetColumn("CD_CLAVE").Hidden = true;
            }
            else if (!string.IsNullOrEmpty(context.GetParameter("agenda")))
            {
                dbQuery = new ConfiguracionReporteQuery(string.Empty, context.GetParameter("agenda"));
                grid.GetColumn("CD_CLAVE").Hidden = true;
            }
            else
                dbQuery = new ConfiguracionReporteQuery();

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                var situacion = row.GetCell("ND_SITUACION").Value;
                if (situacion == CReporte.Anulado)
                    row.DisabledButtons.Add("btnDescargar");
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConfiguracionReporteQuery dbQuery;

            if (!string.IsNullOrEmpty(context.GetParameter("camion")))
            {
                dbQuery = new ConfiguracionReporteQuery(context.GetParameter("camion"), string.Empty);
                grid.GetColumn("CD_CLAVE").Hidden = true;
            }
            else if (!string.IsNullOrEmpty(context.GetParameter("agenda")))
            {
                dbQuery = new ConfiguracionReporteQuery(string.Empty, context.GetParameter("agenda"));
                grid.GetColumn("CD_CLAVE").Hidden = true;
            }
            else
                dbQuery = new ConfiguracionReporteQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConfiguracionReporteQuery dbQuery;

            if (!string.IsNullOrEmpty(context.GetParameter("camion")))
            {
                dbQuery = new ConfiguracionReporteQuery(context.GetParameter("camion"), string.Empty);
                grid.GetColumn("CD_CLAVE").Hidden = true;
            }
            else if (!string.IsNullOrEmpty(context.GetParameter("agenda")))
            {
                dbQuery = new ConfiguracionReporteQuery(string.Empty, context.GetParameter("agenda"));
                grid.GetColumn("CD_CLAVE").Hidden = true;
            }
            else
                dbQuery = new ConfiguracionReporteQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            var nroReporte = long.Parse(context.Row.GetCell("NU_REPORTE").Value);

            if (context.ButtonId == "btnReimprimir")
            {
                this.ReimprimirReporte(nroReporte);

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            }
            else if (context.ButtonId == "btnReprocesar")
            {
                this.ReprocesarReporte(nroReporte);

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }

            return context;
        }

        #region Metodos Auxiliares


        public virtual void ReimprimirReporte(long nroReporte)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var reporte = uow.ReporteRepository.GetReporte(nroReporte);

            if (!reporte.PuedeReimprimir())
                throw new ValidationFailedException("COF070_Sec0_Error_Er004_SitReporteNopermiteReimprimirlo");

            reporte.PrepararReimpresion();

            uow.ReporteRepository.UpdateReporte(reporte);
            uow.SaveChanges();

            if (_taskQueue.IsEnabled() && _taskQueue.IsOnDemandReportProcessing())
            {
                _taskQueue.Enqueue(TaskQueueCategory.REPORT, new List<string> {
                    reporte.Id.ToString()
                });
            }
        }

        public virtual void ReprocesarReporte(long nroReporte)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var reporte = uow.ReporteRepository.GetReporte(nroReporte);

            reporte.PrepararReproceso();

            uow.ReporteRepository.UpdateReporte(reporte);
            uow.SaveChanges();

            if (_taskQueue.IsEnabled() && _taskQueue.IsOnDemandReportProcessing())
            {
                _taskQueue.Enqueue(TaskQueueCategory.REPORT, new List<string> {
                    reporte.Id.ToString()
                });
            }
        }

        #endregion
    }
}
