using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Logic;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
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

namespace WIS.Application.Controllers.REC
{
    public class REC170PanelRecepcion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IDeshacerEmbarqueServiceLegacy _deshacerEmbarqueServiceLegacy;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly Logger _logger;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;
        protected readonly ITrackingService _trackingService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC170PanelRecepcion(
            IIdentityService identity,
            ITrafficOfficerService concurrencyControl,
            IDeshacerEmbarqueServiceLegacy deshacerEmbarqueServiceLegacy,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            ITaskQueueService taskQueue,
            IFactoryService factoryService,
            IParameterService parameterService,
            ITrackingService trackingService)
        {
            this.GridKeys = new List<string>
            {
                "NU_AGENDA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_AGENDA", SortDirection.Descending)
            };

            this._deshacerEmbarqueServiceLegacy = deshacerEmbarqueServiceLegacy;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._concurrencyControl = concurrencyControl;
            this._gridService = gridService;
            this._excelService = excelService;
            this._logger = NLog.LogManager.GetCurrentClassLogger();
            this._security = security;
            this._filterInterpreter = filterInterpreter;
            this._taskQueue = taskQueue;
            this._factoryService = factoryService;
            this._parameterService = parameterService;
            this._trackingService = trackingService;
        }

        public override PageContext PageLoad(PageContext data)
        {
            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", GetBotonesArrayGrid()));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                new GridButton("btnLineas", "General_Sec0_btn_EditarDetalle", "fas fa-list")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PanelRecepcionQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "referencia")?.Value, out int idReferencia))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var listaAgendas = uow.ReferenciaRecepcionRepository.GetNumeroAgendasByReferenciaRecepcion(idReferencia);
                dbQuery = new PanelRecepcionQuery(listaAgendas);
            }
            else
                dbQuery = new PanelRecepcionQuery();

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            ComprobarPermisosEnBotones(uow, grid);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PanelRecepcionQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "referencia")?.Value, out int idReferencia))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new PanelRecepcionQuery(idReferencia);
            }
            else
            {
                dbQuery = new PanelRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_AGENDA", SortDirection.Descending);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PanelRecepcionQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "referencia")?.Value, out int idReferencia))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new PanelRecepcionQuery(idReferencia);
            }
            else
                dbQuery = new PanelRecepcionQuery();

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
                var nuAgenda = context.Row.GetCell("NU_AGENDA").Value;
                var agenda = uow.AgendaRepository.GetAgenda(int.Parse(nuAgenda));

                if (agenda == null)
                    throw new ValidationFailedException("REC170_Frm1_Error_AgendaNoExiste", new string[] { nuAgenda });

                switch (context.ButtonId)
                {
                    case "btnLineas":

                        if (agenda.TipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.Lpn)
                        {
                            context.Parameters.Add(new ComponentParameter("lpn", "true"));
                        }
                        else if (this._concurrencyControl.IsLocked("T_AGENDA", nuAgenda, global: true))
                        {
                            context.Parameters.Add(new ComponentParameter("Lockeada", "true"));
                            throw new ValidationFailedException("REC170_msg_Error_AgendaBloqueada", new string[] { nuAgenda });
                        }
                        break;

                    case "btnDetalleAgenda":
                        context.Redirect("/recepcion/REC171", true, new List<ComponentParameter> {
                            new ComponentParameter("agenda", nuAgenda),
                            new ComponentParameter("cliente", context.Row.GetCell("CD_CLIENTE").Value),
                            new ComponentParameter("empresa", context.Row.GetCell("CD_EMPRESA").Value)
                        });
                        break;

                    case "btnLiberarRecepcion":
                        this.LiberarRecepcion(uow, agenda);
                        context.AddSuccessNotification("REC170_Sec0_Succes_LiberarRecepcion", new List<string> { nuAgenda });
                        break;

                    case "btnCerrarAgenda":

                        if (agenda.PuedeCerrarAgenda())
                        {
                            this.CerrarAgenda(uow, agenda);
                            context.AddSuccessNotification("REC170_Sec0_Succes_CerrarAgenda", new List<string> { nuAgenda });
                        }
                        else
                            context.AddInfoNotification("REC170_Sec0_Error_EstadoAgendaNoPermiteCerrarla");

                        break;

                    case "btnCancelarAgenda":
                        this.CancelarAgenda(uow, agenda);
                        context.AddSuccessNotification("REC170_Sec0_Succes_CancelarAgenda", new List<string> { nuAgenda });

                        break;
                    case "btnAjusteEtiqueta":
                        context.Redirect("/recepcion/REC150", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnGenerarReporte":
                        this.PrepararReportes(uow, agenda);
                        context.AddSuccessNotification("REC170_Sec0_Succes_PrepararReportes", new List<string> { nuAgenda });
                        break;

                    case "btnProblemasRecepcion":
                        context.Redirect("/recepcion/REC141", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnReferencias":
                        context.Redirect("/recepcion/REC010", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnLpns":
                        context.Redirect("/stock/STO700", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnReportes":
                        context.Redirect("/configuracion/COF070", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnCrearCrossDocking":
                        context.Redirect("/recepcion/REC200", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnConsultarCrossDocking":
                        context.Redirect("/recepcion/REC210", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnFinalizarCrossDocking":
                        this.FinalizarCrossDocking(uow, agenda);
                        context.AddSuccessNotification("REC170_Sec0_Msg_CrossDockingFinalizado");
                        break;

                    case "btnDesacerEmbarque":
                        this.DeshacerEmbarque(uow, agenda);
                        context.AddSuccessNotification("REC170_Sec0_Succes_DeshacerEmbarque", new List<string> { nuAgenda });
                        break;

                    case "btnMotivoAlmacenamiento":
                        context.Redirect("/recepcion/REC300", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnVerCalendario":
                        context.Redirect("/recepcion/REC710", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnImprimir":
                        context.Redirect("/recepcion/REC190", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    case "btnEnviarTracking":
                        this.SincronizarTracking(uow, context, agenda);
                        break;

                    case "btnAsociarFacturaAgenda":
                        if (this._concurrencyControl.IsLocked("T_AGENDA", context.Parameters.FirstOrDefault(s => s.Id == "idAgenda").Value))
                        {
                            context.Parameters.Add(new ComponentParameter("Lockeada", "true"));
                            throw new EntityLockedException("REC170_Frm1_Error_EdicionLockeada", new string[] { });
                        }
                        new ComponentParameter() { Id = "keyAgenda", Value = context.Parameters.FirstOrDefault(s => s.Id == "idAgenda").Value };
                        break;
                    case "btnVerFacturas":
                        context.Redirect("/recepcion/REC500", true, new List<ComponentParameter> { new ComponentParameter("agenda", nuAgenda) });
                        break;

                    default:
                        CustomGridButtonAction(uow, context, agenda);
                        break;
                }
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }

            return context;
        }

        #region Metodos generales

        public virtual List<IGridItem> GetBotonesArrayGrid()
        {
            var listaBotones = new List<IGridItem>
            {
                new GridButton("btnDetalleAgenda", "REC170_grid1_btn_btnDetalleAgenda", "fas fa-list"),
                new GridButton("btnLiberarRecepcion", "REC170_grid1_btn_btnLiberarRecepcion", "fas fa-play"),
                new GridButton("btnCerrarAgenda", "REC170_grid1_btn_btnCerrarAgenda", "fas fa-archive"),
                new GridButton("btnCancelarAgenda", "REC170_grid1_btn_btnCancelarAgenda", "fas fa-ban", new ConfirmMessage("REC170_grid1_msg_ConfirmarCancelarAgenda")),
                new GridButton("btnAjusteEtiqueta", "REC170_grid1_btn_ConsultarAjustarEtiqueta", "fas fa-tags"),
                new GridButton("btnGenerarReporte", "REC170_grid1_btn_btnGenerarReporte", "fas fa-file-alt"),
                new GridButton("btnProblemasRecepcion", "REC170_grid1_btn_btnProblemasRecepcion", "fas fa-exclamation-triangle"),
                new GridButton("btnReportes", "REC170_grid1_btn_Reportes", "fas fa-folder-open"),
                new GridButton("btnReferencias", "REC170_grid1_btn_btnReferencias", "fas fa-file-invoice"),
                new GridButton("btnCrearCrossDocking", "REC170_grid1_btn_btnCrearCrossDocking", "fas fa-truck-loading"),
                new GridButton("btnConsultarCrossDocking", "REC170_grid1_btn_btnConsultarCrossDocking", "fas fa-truck-moving"),
                new GridButton("btnFinalizarCrossDocking", "REC170_grid1_btn_btnFinalizarCrossDocking", "fas fa-box"),
                new GridButton("btnDesacerEmbarque", "REC170_grid1_btn_btnDeshacerEmbarque", "fas fa-trash"),
                new GridButton("btnMotivoAlmacenamiento", "REC170_grid1_btn_btnMotivoAlmacenamiento", "fas fa-clipboard-list"),
                new GridButton("btnRecepcionAutomatica", "REC170_grid1_btn_btnRecepcionAutomatica", "fas fa-magic"),
                new GridButton("btnVerCalendario", "WREC170_grid1_btn_btnCalendario", "fas fa-calendar-plus"),
                new GridButton("btnImprimir", "REC170_grid1_btn_btnImprimir", "fas fa-print"),
                new GridButton("btnEnviarTracking", "REC170_grid1_btn_EnviarTracking", "fas fa-share-square"),
                new GridItemDivider(),
                new GridItemHeader("LPN"),
                new GridButton("btnLpns", "REC170_grid1_btn_btnLpns", "fas fa-file-invoice"),
                new GridButton("btnPlanificacionLpns", "REC170_grid1_btn_btnPlanificacionLpns", "fas fa-sheet-plastic"),
                new GridButton("btnRecepcionLpns", "REC170_grid1_btn_btnRecepcionLpns", "fas fa-clipboard"),
                new GridItemDivider(),
                new GridItemHeader("Facturas"),
                new GridButton("btnValidarFacturas", "REC170_grid1_btn_btnValidarFacturas", "fas fa-list"),
                new GridButton("btnAsociarFacturaAgenda", "REC170_grid1_btn_btnAsociarFacturaAgenda", "fas fa-box"),
                new GridButton("btnVerFacturas", "REC170_grid1_btn_btnVerFacturas", "fas fa-list"),
            };

            listaBotones.AddRange(GetCustomGridActions());

            return listaBotones;
        }

        public virtual List<GridButton> GetCustomGridActions()
        {
            return new List<GridButton>
            {
                // new GridButton("btnCustomGridAction", "REC170_grid1_btn_btnCustomGridAction", "fas fa-code")
            };
        }

        public virtual void ComprobarPermisosEnBotones(IUnitOfWork uow, Grid grid)
        {
            Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.REC170_grid1_btn_Editar,
                SecurityResources.REC170Lineas_Page_Access_Allow,
                SecurityResources.WREC170_grid1_btn_DetallesAgenda,
                SecurityResources.WREC170_grid1_btn_LiberarRecepcion,
                SecurityResources.WREC170_grid1_btn_CerrarAgenda,
                SecurityResources.WREC170_grid1_btn_CancelarAgenda,
                SecurityResources.WREC170_grid1_btn_AjusteEtiqueta,
                SecurityResources.WREC170_grid1_btn_DistribucionPedidos,
                SecurityResources.WREC170_grid1_btn_GenerarReporte,
                SecurityResources.REC141_Page_Access_Allow,
                SecurityResources.WREC170_grid1_btn_Reportes,
                SecurityResources.WREC170_grid1_btn_RecepcionFacturaEVO,
                SecurityResources.WREC170_grid1_btn_DeshacerEmbarque,
                SecurityResources.WREC170_grid1_btn_btnMotivoAlmacenamiento,
                SecurityResources.WREC170_grid1_btn_RecepcionAutomatica,
                SecurityResources.WREC710_Page_Access_CalendarioAgendamiento,
                SecurityResources.WREC170_grid1_btn_ImprimirEtiquetas,
                SecurityResources.REC170_grid1_btn_EnviarTracking,
                SecurityResources.REC170_grid1_btn_btnPlanificacionLpns,
                SecurityResources.REC170_grid1_btn_btnRecepcionLpns,
                SecurityResources.WREC170_grid1_btn_AsociarFacturaAgenda,
                SecurityResources.WREC170_grid1_btn_ValidarFacturas,
                SecurityResources.WREC170_grid1_btn_VerFacturas,
            });

            var clientesEntrega = uow.TrackingRepository.GetPuntosEntregaCliente();
            var tiposDeRecepcion = uow.RecepcionTipoRepository.GetTiposRecepcion();

            foreach (var row in grid.Rows)
            {
                int numeroAgenda = int.Parse(row.GetCell("NU_AGENDA").Value);
                var agenda = uow.AgendaRepository.GetAgenda(numeroAgenda);
                var tipoRecepcion = tiposDeRecepcion.FirstOrDefault(t => t.Tipo == agenda.TipoRecepcionInterno);
                var generaReportes = uow.RecepcionTipoRepository.GeneraReporte(agenda.TipoRecepcionInterno, agenda.IdEmpresa);
                var facturaValidada = uow.AgendaRepository.IsAgendaFacturaValida(agenda.Id);
                var facturaAsociadaAgenda = uow.FacturaRepository.AnyFacturaAsociadaAgenda(agenda.Id);

                #region >> btnEditar
                if (!result[SecurityResources.REC170_grid1_btn_Editar] || !agenda.PuedeEditar())
                    row.DisabledButtons.Add("btnEditar");
                #endregion

                #region >> btnLineas
                if (!result[SecurityResources.REC170Lineas_Page_Access_Allow] || !agenda.EnEstadoAbierta() || uow.CrossDockingRepository.AnyAgendaEnCrossDock(numeroAgenda))
                    row.DisabledButtons.Add("btnLineas");
                #endregion

                #region >> btnDetalleAgenda
                if (!result[SecurityResources.WREC170_grid1_btn_DetallesAgenda])
                    row.DisabledButtons.Add("btnDetalleAgenda");
                #endregion

                #region >> btnCerrarAgenda
                if (!result[SecurityResources.WREC170_grid1_btn_CerrarAgenda] || !agenda.PuedeCerrarAgenda())
                    row.DisabledButtons.Add("btnCerrarAgenda");
                #endregion

                #region >> btnCancelarAgenda
                if (!result[SecurityResources.WREC170_grid1_btn_CancelarAgenda] || !agenda.PuedeCancelarAgenda(uow))
                    row.DisabledButtons.Add("btnCancelarAgenda");
                #endregion

                #region >> btnAjusteEtiqueta
                if (result[SecurityResources.WREC170_grid1_btn_AjusteEtiqueta])
                {
                    ConsultaEtiquetasQuery query = new ConsultaEtiquetasQuery();
                    uow.HandleQuery(query);
                    if (!query.AnyEtiquetaAgenda(agenda.Id))
                        row.DisabledButtons.Add("btnAjusteEtiqueta");
                }
                else
                    row.DisabledButtons.Add("btnAjusteEtiqueta");
                #endregion

                #region >> btnGenerarReporte
                if (!result[SecurityResources.WREC170_grid1_btn_GenerarReporte] || (!agenda.EnEstadoCerrada() || !generaReportes))
                    row.DisabledButtons.Add("btnGenerarReporte");
                #endregion

                #region >> btnProblemasRecepcion
                if (!result[SecurityResources.REC141_Page_Access_Allow] || (!agenda.EnEstadoConferidaConDiferencias() || !uow.AgendaRepository.AgendaTuvoProblemas(numeroAgenda))) //|| agenda.EnEstadoCancelada() || agenda.EnEstadoAbierta())
                    row.DisabledButtons.Add("btnProblemasRecepcion");
                #endregion

                #region >> btnReportes 
                if (!result[SecurityResources.WREC170_grid1_btn_Reportes] || !uow.ReporteRepository.AnyReporteAgenda(numeroAgenda))
                    row.DisabledButtons.Add("btnReportes");
                #endregion

                #region >> btnReferencias
                if (!uow.ReferenciaRecepcionRepository.AnyReferenciaEnAgenda(numeroAgenda))
                    row.DisabledButtons.Add("btnReferencias");
                #endregion

                #region >> btnCrearCrossDocking

                if (!CrossDocking.PuedeCrearCrossDock(uow, agenda))
                    row.DisabledButtons.Add("btnCrearCrossDocking");

                #endregion

                #region >> btnConsultarCrossDocking

                if (!uow.CrossDockingRepository.AnyAgendaEnCrossDockConDetalles(numeroAgenda))
                    row.DisabledButtons.Add("btnConsultarCrossDocking");

                #endregion

                #region >> btnFinalizarCrossDocking

                ICrossDocking crossDocking = uow.CrossDockingRepository.GetCrossDockingIniciadoByAgenda(numeroAgenda);

                if (crossDocking == null || !crossDocking.PuedeFinalizarCrossDock())
                    row.DisabledButtons.Add("btnFinalizarCrossDocking");

                #endregion

                #region >> btnDeshacerEmbarque
                if (!result[SecurityResources.WREC170_grid1_btn_DeshacerEmbarque] || !agenda.PuedeDeshacerse())
                    row.DisabledButtons.Add("btnDesacerEmbarque");
                #endregion

                #region >> btnMotivoAlmacenamiento
                // TODO Habilitar cuando se pase la pantalla
                //Redirección REC 300
                /* 
                if (!result[SecurityResources.WREC170_grid1_btn_btnMotivoAlmacenamiento])
                    row.DisabledButtons.Add("btnMotivoAlmacenamiento");
                */
                row.DisabledButtons.Add("btnMotivoAlmacenamiento");

                #endregion

                #region >> btnRecepcionAutomatica

                // TODO Habilitar cuando se desarrolle la funcionalidad
                /*
                if (!result[SecurityResources.WREC170_grid1_btn_RecepcionAutomatica] || agenda.PuedeDeshacerse())
                    if (new RecepcionAutomatica(uow, this._security.UserId, _security.Application, numeroAgenda).PuedeSerRecibidaAutomaicamente())
                        row.DisabledButtons.Add("btnRecepcionAutomatica");
                */
                row.DisabledButtons.Add("btnRecepcionAutomatica");

                #endregion

                #region >> btnCalendarioAgenda
                // TODO habilitar cuando se auste la pantalla
                /*
                if (!result[SecurityResources.WREC710_Page_Access_CalendarioAgendamiento])
                    row.DisabledButtons.Add("btnVerCalendario");
                */
                row.DisabledButtons.Add("btnVerCalendario");

                #endregion

                #region >> btnImprimir
                if (!result[SecurityResources.WREC170_grid1_btn_ImprimirEtiquetas] || (!agenda.EnEstadoConferidaConDiferencias() && !agenda.EnEstadoConferidaSinDiferencias() && !agenda.EnEstadoCerrada()))
                    row.DisabledButtons.Add("btnImprimir");
                #endregion

                #region >> btnEnviarTracking
                if (!result[SecurityResources.REC170_grid1_btn_EnviarTracking] || !_trackingService.TrackingHabilitado()
                    || !agenda.PuedeEnviarTracking(clientesEntrega, tipoRecepcion))
                    row.DisabledButtons.Add("btnEnviarTracking");
                #endregion

                #region >> btnLpns
                if (!uow.ManejoLpnRepository.AnyLpnAsociadoAgenda(numeroAgenda))
                    row.DisabledButtons.Add("btnLpns");
                #endregion

                #region >> btnPlanificacionLpns
                if (!result[SecurityResources.REC170_grid1_btn_btnPlanificacionLpns] || !agenda.EnEstadoAbierta() || tipoRecepcion.PermitePlanificarLpn != "S")
                    row.DisabledButtons.Add("btnPlanificacionLpns");
                #endregion

                #region >> btnRecepcionLpns
                if (!result[SecurityResources.REC170_grid1_btn_btnRecepcionLpns] || !uow.AgendaRepository.AnyPlanificacionLpn(numeroAgenda))
                    row.DisabledButtons.Add("btnRecepcionLpns");
                #endregion

                #region >> Custom Grid Actions
                foreach (var btn in GetCustomGridActions())
                {
                    if (!IsCustomGridActionEnabled(uow, grid, row, btn.Id))
                        row.DisabledButtons.Add(btn.Id);
                }
                #endregion

                #region >> btnAsociarFacturaAgenda
                if (!result[SecurityResources.WREC170_grid1_btn_AsociarFacturaAgenda] || !tipoRecepcion.IngresaFactura || facturaValidada || !agenda.EnEstadoAbierta())
                    row.DisabledButtons.Add("btnAsociarFacturaAgenda");
                #endregion

                #region >> btnValidarFacturas
                if (!result[SecurityResources.WREC170_grid1_btn_ValidarFacturas] || !tipoRecepcion.IngresaFactura || facturaValidada || !agenda.EnEstadoAbierta() || !facturaAsociadaAgenda)
                    row.DisabledButtons.Add("btnValidarFacturas");
                #endregion

                #region >> btnVerFacturas
                if (!result[SecurityResources.WREC170_grid1_btn_VerFacturas] || !tipoRecepcion.IngresaFactura || !facturaAsociadaAgenda)
                    row.DisabledButtons.Add("btnVerFacturas");
                #endregion

                #region >> btnLiberarRecepcion
                if (!result[SecurityResources.WREC170_grid1_btn_LiberarRecepcion] || !agenda.PuedeLiberarse() || (!facturaValidada && tipoRecepcion.IngresaFactura))
                    row.DisabledButtons.Add("btnLiberarRecepcion");
                #endregion
            }
        }

        public virtual bool IsCustomGridActionEnabled(IUnitOfWork uow, Grid grid, GridRow row, string btnId)
        {
            return false;
        }

        public virtual void CustomGridButtonAction(IUnitOfWork uow, GridButtonActionContext data, Agenda agenda)
        {
            // Do nothing
        }

        public virtual void CerrarAgenda(IUnitOfWork uow, Agenda agenda)
        {
            var lAgenda = new LAgenda(uow, _identity.UserId, _identity.Application, _logger);
            var reportes = lAgenda.CerrarAgenda(agenda, _concurrencyControl, _factoryService, _parameterService, _identity);

            if (_taskQueue.IsEnabled() && agenda.NumeroInterfazEjecucion == -1)
                _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionDeRecepcion, agenda.Id.ToString());

            if (_taskQueue.IsEnabled() && _taskQueue.IsOnDemandReportProcessing())
                _taskQueue.Enqueue(TaskQueueCategory.REPORT, reportes.Select(x => x.ToString()).ToList());
        }

        public virtual void CancelarAgenda(IUnitOfWork uow, Agenda agenda)
        {
            uow.CreateTransactionNumber("REC170 Cancelar Agenda");
            uow.BeginTransaction();

            var lAgenda = new LAgenda(uow, _identity.UserId, _identity.Application, _logger);
            lAgenda.CancelarAgenda(agenda, _concurrencyControl);

            uow.Commit();
        }

        public virtual void LiberarRecepcion(IUnitOfWork uow, Agenda agenda)
        {
            uow.CreateTransactionNumber("REC170 Liberar Recepcion");

            var lAgenda = new LAgenda(uow, _identity.UserId, _identity.Application, _logger);
            var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);

            if (agenda.PuedeLiberarse())
            {
                lAgenda.LiberarRecepcion(agenda, _concurrencyControl);
                uow.SaveChanges();
            }
        }

        public virtual void DeshacerEmbarque(IUnitOfWork uow, Agenda agenda)
        {
            uow.CreateTransactionNumber("REC170 Deshacer Embarque");
            var lAgenda = new LAgenda(uow, _identity.UserId, _identity.Application, _logger);
            lAgenda.DeshacerEmbarque(agenda.Id, _deshacerEmbarqueServiceLegacy, _concurrencyControl);
        }

        public virtual void PrepararReportes(IUnitOfWork uow, Agenda agenda)
        {
            var lReportes = new LReportes(uow, _identity.UserId, _identity.Application);
            var predio = agenda.Predio ?? _identity.Predio;
            var reportes = lReportes.PrepararReportes(agenda, predio);

            uow.SaveChanges();

            if (_taskQueue.IsEnabled() && _taskQueue.IsOnDemandReportProcessing())
                _taskQueue.Enqueue(TaskQueueCategory.REPORT, reportes.Select(x => x.ToString()).ToList());
        }

        public virtual void FinalizarCrossDocking(IUnitOfWork uow, Agenda agenda)
        {
            uow.CreateTransactionNumber("REC170 Finalizar Cross Docking");
            uow.BeginTransaction();

            var crossDock = new CrossDockingEnDosFases();
            crossDock.FinalizarCrossDocking(uow, agenda.Id, agenda.IdEmpresa);

            uow.Commit();
        }

        public virtual void SincronizarTracking(IUnitOfWork uow, GridButtonActionContext data, Agenda agenda)
        {
            var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);
            var puntosEntrega = uow.TrackingRepository.GetPuntosEntregaCliente(agenda.CodigoInternoCliente, agenda.IdEmpresa);

            if (!agenda.PuedeEnviarTracking(puntosEntrega, tipoRecepcion))
                throw new ValidationFailedException("REC170_Sec0_Sucess_EstadoEnviarTracking");

            if (puntosEntrega == null)
                throw new ValidationFailedException("REC170_Sec0_Error_ClienteSinPuntosEntregas");
            else if (puntosEntrega.Count == 1)
            {
                var puntoDeEntrega = puntosEntrega.FirstOrDefault();

                _trackingService.SincronizarDevolucion(uow, agenda, puntoDeEntrega.PuntoEntregaPedido);

                uow.CreateTransactionNumber("REC170 SincronizarTracking");

                agenda.NumeroTransaccion = uow.GetTransactionNumber();

                uow.AgendaRepository.UpdateAgenda(agenda);
                uow.SaveChanges();

                data.AddParameter("openTrackingDialog", "N");
                data.AddSuccessNotification("REC170_Sec0_Sucess_DevolucionSincronizada");
            }
            else
                data.AddParameter("openTrackingDialog", "S");
        }

        #endregion
    }
}