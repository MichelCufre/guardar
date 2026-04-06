using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.Logic;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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
    public class STO730DetalleAuditoriaDeLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public STO730DetalleAuditoriaDeLpn(IGridExcelService excelService, IUnitOfWorkFactory uowFactory, IGridService gridService, IIdentityService identity, IFilterInterpreter filterInterpreter, ITaskQueueService taskQueue)
        {
            this.GridKeys = new List<string>
            {
             "NU_AUDITORIA_AGRUPADOR",   "NU_AUDITORIA"
            };

            this.Sorts = new List<SortCommand> {
                new SortCommand("NU_AUDITORIA", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            _excelService = excelService;
            _identity = identity;
            _filterInterpreter = filterInterpreter;
            this._taskQueue = taskQueue;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem>
            {
                new GridButton("BtnRecontar", "STO730_grid1_btn_RecontarLinea"),
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
            var numeroAuditoriaAgrupado = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "numeroAuditoriaAgrupado").Value);

            var dbQuery = new ConsultaDetalleAuditoriaLpnQuery(numeroAuditoriaAgrupado);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);

            var estado = uow.ManejoLpnRepository.GetEstadoAuditoria(numeroAuditoriaAgrupado);
            foreach (var row in grid.Rows)
            {
                if (estado != EstadoAuditoriaLpn.Pendiente && estado != EstadoAuditoriaLpn.Recontar)
                    row.DisabledSelected = true;
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var numeroAuditoriaAgrupado = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "numeroAuditoriaAgrupado").Value);

            var dbQuery = new ConsultaDetalleAuditoriaLpnQuery(numeroAuditoriaAgrupado);
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
            var numeroAuditoriaAgrupado = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "numeroAuditoriaAgrupado").Value);

            var dbQuery = new ConsultaDetalleAuditoriaLpnQuery(numeroAuditoriaAgrupado);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "STO730Detalle_grid_1" && context.ButtonId == "BtnRecontar")
            {
                using var uow = this._uowFactory.GetUnitOfWork();
                uow.CreateTransactionNumber("Marcar Recontar Auditoria");
                uow.BeginTransaction();

                var detalles = GetSelectedDetalleAuditados(uow, context);
                foreach (var det in detalles)
                {
                    long nuAuditoria = long.Parse(det[1]);
                    var auditoria = uow.ManejoLpnRepository.GetAuditoria(nuAuditoria);

                    auditoria.Estado = EstadoAuditoriaLpn.Recontar;
                    auditoria.Transaccion = uow.GetTransactionNumber();
                    auditoria.FechaModificacion = DateTime.Now;
                    auditoria.FuncionarioModificacionEstado = this._identity.UserId;

                    uow.ManejoLpnRepository.UpdateAuditoriaLpn(auditoria);
                    uow.SaveChanges();
                }

                uow.Commit();
                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var numeroAuditoriaAgrupado = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "numeroAuditoriaAgrupado").Value);

            var estado = uow.ManejoLpnRepository.GetEstadoAuditoria(numeroAuditoriaAgrupado);

            if (estado == EstadoAuditoriaLpn.Pendiente)
                context.Parameters.Add(new ComponentParameter { Id = "isDisableButton", Value = "F" });
            else
                context.Parameters.Add(new ComponentParameter { Id = "isDisableButton", Value = "T" });

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                var listKey = new List<string>();

                var numeroAuditoriaAgrupado = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "numeroAuditoriaAgrupado").Value);

                var detallesAuditoria = uow.ManejoLpnRepository.GetDetallesAuditadosAgrupados(numeroAuditoriaAgrupado)
                    .Where(d => d.Estado == EstadoAuditoriaLpn.Pendiente)
                    .ToList();

                var estado = uow.ManejoLpnRepository.GetEstadoAuditoria(numeroAuditoriaAgrupado);
                if (estado != EstadoAuditoriaLpn.Pendiente)
                {
                    context.AddInfoNotification("STO730_Sec0_Error_AuditoriaNoPermiteRealizarOperacion");
                    return form;
                }


                uow.BeginTransaction();
                var rechazarAuditoria = context.ButtonId == "btnRechazar";

                if (context.ButtonId == "btnRechazar")
                    uow.CreateTransactionNumber("Rechazar Auditoría");
                else
                    uow.CreateTransactionNumber("Aprobar Auditoría");

                foreach (var detAditoria in detallesAuditoria)
                {
                    if (rechazarAuditoria)
                    {
                        detAditoria.Estado = EstadoAuditoriaLpn.Cancelada;
                        detAditoria.FuncionarioModificacion = this._identity.UserId;
                        detAditoria.Transaccion = uow.GetTransactionNumber();
                        detAditoria.FechaModificacion = DateTime.Now;
                        detAditoria.FuncionarioModificacionEstado = this._identity.UserId;

                        uow.ManejoLpnRepository.UpdateAuditoriaLpn(detAditoria);
                        uow.SaveChanges();
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

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

                if (_taskQueue.IsEnabled() && listKey.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, listKey);
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return form;
        }
        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            return base.FormButtonAction(form, context);
        }

        public virtual List<string[]> GetSelectedDetalleAuditados(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            long numeroAuditoriaAgrupado = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "numeroAuditoriaAgrupado").Value);

            var dbQuery = new ConsultaDetalleAuditoriaLpnQuery(numeroAuditoriaAgrupado);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }
    }
}