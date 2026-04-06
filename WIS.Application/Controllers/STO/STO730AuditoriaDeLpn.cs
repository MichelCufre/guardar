using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.Logic;
using WIS.Domain.Services.Interfaces;
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
    public class STO730AuditoriaDeLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly Logger _logger;
        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public STO730AuditoriaDeLpn(IGridExcelService excelService, IUnitOfWorkFactory uowFactory, IGridService gridService, IIdentityService identity, IFilterInterpreter filterInterpreter, ITaskQueueService taskQueue)
        {
            this.GridKeys = new List<string>
            {
                "NU_AUDITORIA_AGRUPADOR"
            };

            this.Sorts = new List<SortCommand> {
                new SortCommand("NU_AUDITORIA_AGRUPADOR", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._identity = identity;
            this._filterInterpreter = filterInterpreter;
            this._taskQueue = taskQueue;
            this._logger = LogManager.GetCurrentClassLogger();
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem>
            {
                new GridButton("BtnAprobar", "STO730_grid1_btn_AprobarLineas"),
                new GridButton("btnRechazar", "STO730_grid1_btn_RechazarLineas")
            };

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnLineas", "STO700_Sec0_btn_AtributosAuditados", "fas fa-list")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ConsultaAuditoriaLpnQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                if (row.GetCell("ID_ESTADO").Value != EstadoAuditoriaLpn.Pendiente)
                    row.DisabledSelected = true;
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ConsultaAuditoriaLpnQuery();
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

            var dbQuery = new ConsultaAuditoriaLpnQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var recahzarAuditoria = context.ButtonId == "btnRechazar";
                var operacionParcial = false;
                var listKey = new List<string>();

                if (recahzarAuditoria)
                    uow.CreateTransactionNumber("Rechazar Auditoría");
                else
                    uow.CreateTransactionNumber("Aprobar Auditoría");

                foreach (var detAuditoriaAgrupado in GetSelectedLineas(uow, context))
                {
                    var detallesAuditoria = uow.ManejoLpnRepository.GetDetallesAuditadosAgrupados(long.Parse(detAuditoriaAgrupado[0]));
                    if (detallesAuditoria.Any(x => x.Estado == EstadoAuditoriaLpn.Recontar))
                    {
                        operacionParcial = true;
                        continue;
                    }

                    foreach (var detAditoria in detallesAuditoria.Where(x => x.Estado == EstadoAuditoriaLpn.Pendiente).ToList())
                    {
                        if (recahzarAuditoria)
                        {
                            detAditoria.Estado = EstadoAuditoriaLpn.Cancelada;
                            detAditoria.Transaccion = uow.GetTransactionNumber();
                            detAditoria.FechaModificacion = DateTime.Now;
                            detAditoria.FuncionarioModificacion = this._identity.UserId;
                            detAditoria.FuncionarioModificacionEstado = this._identity.UserId;

                            uow.ManejoLpnRepository.UpdateAuditoriaLpn(detAditoria);
                        }
                        else
                        {
                            var lpn = uow.ManejoLpnRepository.GetLpn(detAditoria.Lpn);
                            var lpnTipoLpn = uow.ManejoLpnRepository.GetTipoLpn(lpn.Tipo);
                            var lpnBarras = uow.ManejoLpnRepository.GetCodigoDeBarras(lpn.NumeroLPN);
                            var areaUbicacion = uow.UbicacionAreaRepository.GetAreaByUbicacion(lpn.Ubicacion);

                            var logicLpn = new LManejoLpn(_identity, _logger);
                            logicLpn.AprobarAuditoriaLpn(uow, listKey, detAditoria, lpn, lpnTipoLpn, lpnBarras, areaUbicacion);

                            detAditoria.Estado = EstadoAuditoriaLpn.Aprobada;
                            detAditoria.FechaModificacion = DateTime.Now;
                            detAditoria.Transaccion = uow.GetTransactionNumber();
                            detAditoria.FuncionarioModificacionEstado = this._identity.UserId;

                            uow.ManejoLpnRepository.UpdateAuditoriaLpn(detAditoria);
                            uow.SaveChanges();
                        }
                    }

                    var det = detallesAuditoria.FirstOrDefault();
                    var detallesLpn = uow.ManejoLpnRepository.GetDetallesLpn(det.Lpn, null, det.Producto, det.Empresa, det.Identificador, det.Faixa);

                    foreach (var detalleLpn in detallesLpn)
                    {
                        detalleLpn.IdInventario = "R";
                        detalleLpn.NumeroTransaccion = uow.GetTransactionNumber();

                        uow.ManejoLpnRepository.UpdateDetalleLpn(detalleLpn);
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                if (_taskQueue.IsEnabled() && listKey.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, listKey);

                if (!operacionParcial)
                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                else
                    context.AddInfoNotification("PAR401_Sec0_Error_ErrorOperacionParcial");
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return context;
        }

        public virtual List<string[]> GetSelectedLineas(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new ConsultaAuditoriaLpnQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }
    }
}