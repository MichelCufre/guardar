using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.Facturacion;
using WIS.Domain.Reportes;
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
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.EXP
{
    public class EXP040PanelEgreso : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IParameterService _parameterService;
        protected readonly IReportKeyService _reporteKeyService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrackingService _trackingService;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly IFactoryService _factoryService;
        protected readonly IDapper _dapper;
        protected readonly ILogger<EXP040PanelEgreso> _logger;
        protected readonly IBarcodeService _barcodeService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP040PanelEgreso(
            IIdentityService identity,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IParameterService parameterService,
            IReportKeyService reporteKeyService,
            ISecurityService security,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrackingService trackingService,
            ITaskQueueService taskQueue,
            IFactoryService factoryService,
            ILogger<EXP040PanelEgreso> logger,
            IDapper dapper,
            IBarcodeService barcodeService)
        {
            this.GridKeys = new List<string>
            {
                "CD_CAMION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_CAMION",SortDirection.Descending)
            };

            this._identity = identity;
            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._parameterService = parameterService;
            this._reporteKeyService = reporteKeyService;
            this._security = security;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._trackingService = trackingService;
            this._taskQueue = taskQueue;
            this._factoryService = factoryService;
            this._dapper = dapper;
            this._logger = logger;
            this._barcodeService = barcodeService;
        }

        public override PageContext PageLoad(PageContext data)
        {
            var keyPlanificacion = SecurityResources.EXP040CreatePlanificacion_Page_Access_Allow;
            var listaPermisos = new List<string>()
            {
                SecurityResources.EXP040CreateEgreso_Page_Access_Allow,
                keyPlanificacion,
            };

            var resultados = this._security.CheckPermissions(listaPermisos);

            foreach (var res in resultados.Where(r => r.Key != keyPlanificacion))
            {
                data.Parameters.Add(new ComponentParameter(res.Key, res.Value.ToString()));
            }

            bool habilitado = resultados[keyPlanificacion] ? _trackingService.TrackingHabilitado() : false;
            data.Parameters.Add(new ComponentParameter(keyPlanificacion, habilitado.ToString()));

            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                new GridButton("btnCerrarCamion", "EXP040_grid1_btn_CerrarCamion"),
                new GridButton("btnCancelarCamion", "EXP040_grid1_btn_CancelarCamion"),
                new GridButton("btnDetalleCamion", "EXP040_grid1_btn_DetalleCamion"),
                new GridButton("btnFacturarCamion", "EXP040_grid1_btn_FacturarCamion"),
                new GridButton("btnEgresoDocumental", "EXP040_grid1_btn_EgresoDocumental"),
                new GridButton("btnArmarCamion", "WEXP040_grid1_btn_ArmarEgreso"),
                new GridButton("btnArmarCamionCont", "WEXP040_grid1_btn_ArmarEgresoCont"),
                new GridButton("btnArmarCamionEntrega", "WEXP040_grid1_btn_ArmarEgresoEntrega"),
                new GridButton("btnArmarCamionPedido", "WEXP040_grid1_btn_ArmarEgresoPedido"),
                new GridButton("btnPedidosInvolucreados", "WEXP040_grid1_btn_PedidosInvolucrados"),
                new GridButton("btnPedidosExpedidos", "WEXP040_grid1_btn_PedidosExpedidos"),
                new GridButton("btnPedidosPendientesPorCamion", "WEXP040_grid1_btn_PedidosPendientesCamion"),
                new GridButton("btnExcluirCargasNoPreparadas", "WEXP040_grid1_btn_ExcluirCargasNoPreparadas"),
                new GridButton("btnGenerarReportes", "WEXP040_grid1_btn_btnGenerarReportes"),
                new GridButton("btnReportes", "WEXP040_grid1_btn_Reportes"),
                new GridItemDivider(),
                new GridItemHeader("Tracking"),
                new GridButton("btnPuntosEntrega", "WEXP040_grid1_btn_PuntosEntregaCamion"),
                new GridButton("btnSincronizarTracking", "WEXP040_grid1_btn_SincronizarTracking"),
                new GridButton("btnReSincronizarTracking", "EXP040_grid1_btn_ReSincronizarTracking")
            }));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_EDITAR", new List<GridButton>
            {
                new GridButton("btnEditar", "EXP040_grid1_btn_Editar", "fas fa-edit")
            }));

            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            int? cd_camion = null;
            string sesionCamion = context.GetParameter("CD_CAMION");

            if (!string.IsNullOrEmpty(sesionCamion))
                cd_camion = int.Parse(sesionCamion);

            var dbQuery = new CamionesExpedicionQuery(cd_camion);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            HabilitarBotones(uow, grid);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new CamionesExpedicionQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? cd_camion = null;
            string sesionCamion = context.GetParameter("CD_CAMION");

            if (!string.IsNullOrEmpty(sesionCamion))
                cd_camion = int.Parse(sesionCamion);

            var dbQuery = new CamionesExpedicionQuery(cd_camion);

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

            try
            {
                if (context.ButtonId == "btnCerrarCamion")
                    this.AccionCerrarCamion(uow, context);

                if (context.ButtonId == "btnCancelarCamion")
                    this.AccionCancelarCamion(uow, context);

                if (context.ButtonId == "btnEditar")
                    this.AccionEditarCamion(uow, context);

                if (context.ButtonId == "btnFacturarCamion")
                    this.FacturarCamion(uow, context);

                if (context.ButtonId == "btnEgresoDocumental")
                {
                    context.Redirect("/expedicion/EXP052", new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "camion", Value = context.Row.GetCell("CD_CAMION").Value},
                        new ComponentParameter(){ Id = "respetaOrden", Value = context.Row.GetCell("ID_RESPETA_ORD_CARGA").Value},
                    });
                }

                if (context.ButtonId.StartsWith("btnArmarCamion"))
                {
                    string camion = context.Row.GetCell("CD_CAMION").Value;

                    if (this._concurrencyControl.IsLocked("T_CAMION", camion))
                        throw new EntityLockedException("Registro bloqueado");
                }

                if (context.ButtonId == "btnPedidosInvolucreados")
                {
                    context.Redirect("/expedicion/EXP050", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "camion", Value = context.Row.GetCell("CD_CAMION").Value},
                        new ComponentParameter(){ Id = "respetaOrden", Value = context.Row.GetCell("ID_RESPETA_ORD_CARGA").Value},
                    });
                }

                if (context.ButtonId == "btnPedidosExpedidos")
                {
                    context.Redirect("/expedicion/EXP041", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "camion", Value = context.Row.GetCell("CD_CAMION").Value},
                    });
                }

                if (context.ButtonId == "btnPedidosPendientesPorCamion")
                {
                    context.Redirect("/expedicion/EXP043", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "camion", Value = context.Row.GetCell("CD_CAMION").Value},
                    });
                }

                if (context.ButtonId == "btnGenerarReportes")
                {
                    uow.BeginTransaction();

                    var camion = uow.CamionRepository.GetCamionWithCargas(int.Parse(context.Row.GetCell("CD_CAMION").Value));
                    var cierre = new CierreEgreso(uow, camion, _dapper, this._parameterService, this._identity, this._factoryService, _reporteKeyService, _barcodeService, _taskQueue);
                    var reportes = cierre.GenerarReportes(uow, camion);

                    uow.Commit();

                    if (_taskQueue.IsEnabled() && _taskQueue.IsOnDemandReportProcessing())
                        _taskQueue.Enqueue(TaskQueueCategory.REPORT, reportes.Select(x => x.ToString()).ToList());

                    context.AddSuccessNotification("EXP040_Sec0_error_ReportesGenerados");
                }

                if (context.ButtonId == "btnReportes")
                {
                    context.Redirect("/configuracion/COF070", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter(){Id = "camion", Value = context.Row.GetCell("CD_CAMION").Value}
                    });
                }

                if (context.ButtonId == "btnDetalleCamion")
                {
                    context.Redirect("/expedicion/EXP045", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter(){Id = "camion", Value = context.Row.GetCell("CD_CAMION").Value}
                    });
                }

                if (context.ButtonId == "btnSincronizarTracking")
                {
                    uow.BeginTransaction();
                    uow.CreateTransactionNumber("SincronizarTracking");

                    Camion camion = uow.CamionRepository.GetCamionWithCargas(int.Parse(context.Row.GetCell("CD_CAMION").Value));

                    SincronizarTracking(uow, context, camion, false, true);

                    uow.Commit();

                    context.AddSuccessNotification("EXP040_Success_msg_CamionSincronizado");
                }

                if (context.ButtonId == "btnReSincronizarTracking")
                {
                    uow.BeginTransaction();
                    uow.CreateTransactionNumber("ReSincronizarTracking");

                    Camion camion = uow.CamionRepository.GetCamionWithCargas(int.Parse(context.Row.GetCell("CD_CAMION").Value));

                    SincronizarTracking(uow, context, camion, true, true);

                    uow.Commit();

                    context.AddSuccessNotification("EXP040_Success_msg_CamionSincronizado");
                }
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));

                uow.Rollback();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message);

                uow.Rollback();
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual void HabilitarBotones(IUnitOfWork uow, Grid grid)
        {
            Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.WEXP040_grid1_btn_CerrarCamion,
                SecurityResources.WEXP040_grid1_btn_CancelarCamion,
                SecurityResources.WEXP040_grid1_btn_DetallesCamion,
                SecurityResources.WEXP040_grid1_btn_PedidosInvolucrados,
                SecurityResources.WEXP040_grid1_btn_PedidosExpedidos,
                SecurityResources.WEXP040_grid1_btn_FacturarCamion,
                SecurityResources.EXP052_Page_Access_Allow,
                SecurityResources.WEXP040_grid1_btn_PedidosPendientesCamion,
                SecurityResources.WEXP040_grid1_btn_ArmarCamion,
                SecurityResources.WEXP040_grid1_btn_ArmarCamionCont,
                SecurityResources.WEXP040_grid1_btn_ArmarCamionEntrega,
                SecurityResources.WEXP013_Page_Access_ArmarEgresoPedido,
                SecurityResources.WEXP040_grid1_btn_PuntosEntregaCamion,
                SecurityResources.WEXP040_grid1_btn_ExcluirCargasNoPreparadas,
                SecurityResources.WEXP040_grid1_btn_GenerarReportes,
                SecurityResources.WEXP040_grid1_btn_Reportes,
                SecurityResources.WEXP040_grid1_btn_SincronizarTracking,
                SecurityResources.WEXP040_grid1_btn_ReSincronizarTracking
            });

            var egresosAMarcar = uow.CamionRepository.GetEgresosAMarcar();

            foreach (var row in grid.Rows)
            {
                var camionValidation = new EgresoButtonValidation(this._parameterService, uow);

                int cdCamion = int.Parse(row.GetCell("CD_CAMION").Value);

                camionValidation.LoadCamion(cdCamion, row.GetCell("CD_SITUACAO").Value, row.GetCell("NU_INTERFAZ_EJECUCION_FACT").Value, row.GetCell("FL_TRACKING").Value,
                    row.GetCell("FL_SYNC_REALIZADA").Value, row.GetCell("FL_CONF_VIAJE_REALIZADA").Value, row.GetCell("DS_DOCUMENTO").Value, row.GetCell("DT_FACTURACION").Value, row.GetCell("TIENE_PEDIDOS_PENDIENTES").Value,
                    row.GetCell("CD_EMPRESA").Value, row.GetCell("TP_ARMADO_EGRESO").Value);

                if (!result[SecurityResources.WEXP040_grid1_btn_CerrarCamion] || camionValidation.IsCerrado() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnCerrarCamion");

                if (!result[SecurityResources.WEXP040_grid1_btn_CancelarCamion] || camionValidation.IsCerrado() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnCancelarCamion");

                if (!result[SecurityResources.WEXP040_grid1_btn_DetallesCamion])
                    row.DisabledButtons.Add("btnDetalleCamion");

                if (!result[SecurityResources.WEXP040_grid1_btn_PedidosInvolucrados] || camionValidation.IsCerrado() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnPedidosInvolucreados");

                if (!result[SecurityResources.WEXP040_grid1_btn_PedidosExpedidos] || !camionValidation.IsCerrado() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnPedidosExpedidos");

                if (!result[SecurityResources.WEXP040_grid1_btn_FacturarCamion] || !camionValidation.PuedeFacturarse() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnFacturarCamion");

                if (!result[SecurityResources.EXP052_Page_Access_Allow] || !camionValidation.PuedeGenerarseEgresoDocumental() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnEgresoDocumental");

                if (!result[SecurityResources.WEXP040_grid1_btn_ArmarCamion] || !camionValidation.PuedeArmarsePorCarga())
                    row.DisabledButtons.Add("btnArmarCamion");

                if (!result[SecurityResources.WEXP040_grid1_btn_ArmarCamionCont] || !camionValidation.PuedeArmarse(armadoPorContenedor: true))
                    row.DisabledButtons.Add("btnArmarCamionCont");

                if (!result[SecurityResources.WEXP040_grid1_btn_ArmarCamionEntrega] || !camionValidation.PuedeArmarse() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnArmarCamionEntrega");

                if (!result[SecurityResources.WEXP013_Page_Access_ArmarEgresoPedido] || !camionValidation.PuedeArmarse())
                    row.DisabledButtons.Add("btnArmarCamionPedido");

                if (!result[SecurityResources.WEXP040_grid1_btn_PedidosPendientesCamion] || camionValidation.IsCerrado() || !camionValidation.TienePedidosPendientes() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnPedidosPendientesPorCamion");

                if (!result[SecurityResources.WEXP040_grid1_btn_ExcluirCargasNoPreparadas] || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnExcluirCargasNoPreparadas");

                if (!result[SecurityResources.WEXP040_grid1_btn_GenerarReportes] || !camionValidation.IsCerrado() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnGenerarReportes");

                if (!result[SecurityResources.WEXP040_grid1_btn_Reportes] || !camionValidation.IsCerrado() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnReportes");

                if (!result[SecurityResources.WEXP040_grid1_btn_PuntosEntregaCamion] || !camionValidation.PuedeArmarse() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnPuntosEntrega");

                if (!result[SecurityResources.WEXP040_grid1_btn_SincronizarTracking] || !_trackingService.TrackingHabilitado() || !camionValidation.PuedeSincronizarTracking())
                    row.DisabledButtons.Add("btnSincronizarTracking");

                if (camionValidation.IsCerrado())
                    row.DisabledButtons.Add("btnEditar");

                if (!result[SecurityResources.WEXP040_grid1_btn_ReSincronizarTracking] || !_trackingService.TrackingHabilitado() || !camionValidation.PuedeReSincronizarTracking() || camionValidation.IsPlanificacion())
                    row.DisabledButtons.Add("btnReSincronizarTracking");

                if (_trackingService.TrackingHabilitado() && egresosAMarcar.Contains(cdCamion))
                    row.CssClass = "yellow";
            }
        }

        public virtual void AccionEditarCamion(IUnitOfWork uow, GridButtonActionContext context)
        {
            int cdCamion = Convert.ToInt32(context.Row.GetCell("CD_CAMION").Value);
            Camion camion = uow.CamionRepository.GetCamion(cdCamion);

            if (camion.IsCerrado())
                throw new ValidationFailedException("Camion se encuentra cerrado. No puede modificarse");
        }

        public virtual void FacturarCamion(IUnitOfWork uow, GridButtonActionContext context)
        {
            uow.BeginTransaction();
            uow.CreateTransactionNumber("FacturarCamion");
            var transaction = this._concurrencyControl.CreateTransaccion();

            try
            {
                int cdCamion = Convert.ToInt32(context.Row.GetCell("CD_CAMION").Value);
                Camion camion = uow.CamionRepository.GetCamionWithCargas(cdCamion);

                if (_concurrencyControl.IsLocked("T_CAMION", camion.Id.ToString(), true))
                    throw new ValidationFailedException("EXP040_msg_Error_CamionBloqueado");

                _concurrencyControl.AddLock("T_CAMION", camion.Id.ToString(), transaction, true);

                var formatResolver = new ValidacionFacturacionResultFormatResolver();
                var configuracion = new ConfiguracionValidacionFacturacion(uow, formatResolver);
                var validacion = new ValidacionFacturacionCamion(uow, configuracion);

                var facturacion = new FacturacionCamion(uow, validacion, camion);

                List<ValidacionCamionResultado> resultado = facturacion.Facturar();

                if (resultado.Any())
                {
                    uow.Rollback();
                    context.AddParameter("resultadoValidacion", JsonConvert.SerializeObject(resultado));
                    return;
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("EXP040_Success_msg_EgresoFacturado");

                if (_taskQueue.IsEnabled() && camion.NumeroInterfazEjecucionFactura == -1)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.Facturacion, camion.Id.ToString());
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transaction);
            }
        }

        public virtual void AccionCerrarCamion(IUnitOfWork uow, GridButtonActionContext context)
        {
            var reportes = new List<long>();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("CerrarCamion");

            int cdCamion = Convert.ToInt32(context.Row.GetCell("CD_CAMION").Value);

            Camion camion = uow.CamionRepository.GetCamionWithCargas(cdCamion);
            var transaction = this._concurrencyControl.CreateTransaccion();

            try
            {
                if (_concurrencyControl.IsLocked("T_CAMION", camion.Id.ToString()))
                    throw new ValidationFailedException("EXP040_msg_Error_CamionBloqueado");

                _concurrencyControl.AddLock("T_CAMION", camion.Id.ToString(), transaction, true);

                var resultado = CerrarCamion(uow, context, camion, out reportes);

                if (resultado.Any())
                {
                    uow.Rollback();
                    context.AddParameter("resultadoValidacion", JsonConvert.SerializeObject(resultado));
                    return;
                }

                uow.Commit();

                context.AddSuccessNotification("EXP040_Success_msg_EgresoCerrado");

                if (_taskQueue.IsEnabled() && camion.NumeroInterfazEjecucionCierre == -1)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionDePedido, camion.Id.ToString());

                if (_taskQueue.IsEnabled() && _taskQueue.IsOnDemandReportProcessing())
                    _taskQueue.Enqueue(TaskQueueCategory.REPORT, reportes.Select(x => x.ToString()).ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError($"AccionCerrarCamion Error: {ex.Message}");
                throw ex;
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transaction);
            }
        }

        public virtual List<ValidacionCamionResultado> CerrarCamion(IUnitOfWork uow, GridButtonActionContext context, Camion camion, out List<long> reportes)
        {
            reportes = new List<long>();

            try
            {
                var cierre = new CierreEgreso(uow, camion, _dapper, this._parameterService, this._identity, this._factoryService, this._reporteKeyService, _barcodeService, _taskQueue);
                var resultadoValidacion = cierre.Cerrar();

                if (resultadoValidacion.Any())
                    return resultadoValidacion;

                cierre.ExpedirEnvases(uow, camion);

                reportes = cierre.GenerarReportes(uow, camion);

                uow.SaveChanges();

                SincronizarTracking(uow, context, camion, true, false);
            }
            catch (Exception ex)
            {
                this._logger.LogError($"CerrarCamion Error: {ex.Message}");
                throw ex;
            }

            return new List<ValidacionCamionResultado>();
        }

        public virtual void ValidarSincronizacion(IUnitOfWork uow, GridButtonActionContext context, Camion camion, ref bool bloquear)
        {
            var resultado = _trackingService.ValidarSincronizacion(uow, camion);
            if (resultado != null)
            {
                context.AddParameter("resultadoValidacion", JsonConvert.SerializeObject(resultado));
                bloquear = true;
                _trackingService.GuardarErrores(_uowFactory, $"SincronizarViaje ValidarPedidosNoManejanTracking - CD_CAMION: {camion.Id}", $"Pedidos que no manejan tracking: {JsonConvert.SerializeObject(resultado.FirstOrDefault().Datos)}");
                throw new Exception("");
            }
        }

        public virtual void SincronizarTracking(IUnitOfWork uow, GridButtonActionContext context, Camion camion, bool confirmarViaje, bool bloquear)
        {
            try
            {
                if (_trackingService.TrackingHabilitado())
                {
                    if (camion.IsTrackingHabilitado)
                        ValidarSincronizacion(uow, context, camion, ref bloquear);

                    if (camion.TipoArmadoEgreso != TipoArmadoEgreso.Planificacion)
                        _trackingService.SincronizarEgreso(uow, camion, confirmarViaje);
                    else
                        _trackingService.SincronizarPlanificacion(uow, camion);

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError($"SincronizarTracking Error: {ex.Message}");
                if (bloquear)
                    throw ex;
            }
        }

        public virtual void AccionCancelarCamion(UnitOfWork uow, GridButtonActionContext context)
        {
            uow.BeginTransaction();
            uow.CreateTransactionNumber("CancelarCamion");

            int cdCamion = Convert.ToInt32(context.Row.GetCell("CD_CAMION").Value);

            Camion camion = uow.CamionRepository.GetCamionWithCargas(cdCamion);

            if (camion == null)
                throw new ValidationFailedException("General_Sec0_Error_Er100_CamionNoExiste");

            if (uow.ContenedorRepository.AnyContenedorCargadoEnCamion(cdCamion))
                throw new ValidationFailedException("EXP040_msg_Error_CamionConContenedoresCargados");

            var transaction = this._concurrencyControl.CreateTransaccion();

            try
            {
                if (_concurrencyControl.IsLocked("T_CAMION", camion.Id.ToString()))
                    throw new ValidationFailedException("EXP040_msg_Error_CamionBloqueado");

                _concurrencyControl.AddLock("T_CAMION", camion.Id.ToString(), transaction, true);

                if (camion.Cargas.Any())
                    CancelarCamionConCargas(uow, context, camion, camion.Cargas);
                else
                    CancelarCamion(uow, context, camion);

                uow.Commit();

                context.AddSuccessNotification("EXP040_Success_msg_EgresoCancelado");

                if (_taskQueue.IsEnabled() && camion.NumeroInterfazEjecucionCierre == -1)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionDePedido, camion.Id.ToString());
            }
            catch (ValidationFailedException)
            {
                uow.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                uow.Rollback();
                _logger.LogError($"AccionCancelarCamion Error: {ex.Message}");
                throw;
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transaction);
            }
        }

        public virtual void CancelarCamionConCargas(UnitOfWork uow, GridButtonActionContext context, Camion camion, List<CargaCamion> cargas)
        {
            try
            {
                var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());

                var manejoDocumentalActivo = expedicionService.IsManejoDocumentalHabilitado(camion.Empresa ?? -1);

                IDocumentoEgreso documentoEgreso = null;
                bool egresoDocumentalEditable = false;

                if (manejoDocumentalActivo)
                {
                    documentoEgreso = uow.DocumentoRepository.GetEgresoPorCamion(camion.Id);

                    if (documentoEgreso != null)
                        egresoDocumentalEditable = uow.DocumentoTipoRepository.PermiteEditarCamion(documentoEgreso.Tipo, documentoEgreso.Estado);
                    else
                        egresoDocumentalEditable = true;
                }

                if (!camion.PuedeArmarse(manejoDocumentalActivo, egresoDocumentalEditable, armadoPorContenedor: true))
                {
                    if (camion.NumeroInterfazEjecucionFactura == -1)
                        throw new ValidationFailedException("WEXP010_Sec0_Error_Er004_CamionPendienteFacturarInmodificable");
                    else if (manejoDocumentalActivo && !egresoDocumentalEditable)
                        throw new ValidationFailedException("General_Sec0_Error_EgresoDocumentalNoEditable");
                    else
                        throw new ValidationFailedException("General_Sec0_Error_EstadoCamionNoArmable");
                }

                if (manejoDocumentalActivo && documentoEgreso != null)
                {
                    var estadoDestino = uow.DocumentoRepository.GetEstadoDestino(documentoEgreso.Tipo, AccionDocumento.Cancelar);

                    if (estadoDestino == null)
                        throw new ValidationFailedException("General_Sec0_Error_ImposbileCancelarEgreso");

                    documentoEgreso.Estado = estadoDestino;
                    documentoEgreso.Cancelar();

                    uow.DocumentoRepository.UpdateEgreso(documentoEgreso, uow.GetTransactionNumber());

                    camion.Documento = null;

                    uow.SaveChanges();
                }

                uow.CamionRepository.RemoveCargasCamion(camion.Id, cargas);
                camion.Cargas.Clear();

                uow.SaveChanges();

                CancelarCamion(uow, context, camion);
            }
            catch (ValidationFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"CancelarCamionConCargas Error: {ex.Message}");
                throw;
            }
        }

        public virtual void CancelarCamion(UnitOfWork uow, GridButtonActionContext context, Camion camion)
        {
            try
            {
                camion.Cerrar();
                uow.CamionRepository.UpdateCamion(camion);
                uow.SaveChanges();

                SincronizarTracking(uow, context, camion, true, false);
            }
            catch (Exception ex)
            {
                this._logger.LogError($"CancelarCamion Error: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}
