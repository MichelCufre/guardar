using Dispatch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartFormat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Eventos;
using WIS.Domain.General;
using WIS.Domain.General.API;
using WIS.Domain.Interfaces;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Http;

namespace WIS.Domain.Services
{
    public class TaskQueueService : ITaskQueueService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IWebApiClient _client;
        protected readonly IOptions<TaskQueueSettings> _configuration;
        protected readonly ILogger<TaskQueueService> _logger;
        protected readonly IEjecucionService _ejecucionService;
        protected readonly IEmpresaService _empresaService;
        protected readonly IParameterService _parameterService;
        protected readonly IWebhookCallerService _webhookCallerService;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected int _timeout = 30;
        protected int _retrySleep = 5;
        protected int _maxRetries = 5;
        protected bool? _isOnDemandReportProcessing;
        protected static readonly SerialQueue Queue = new SerialQueue();

        public TaskQueueService(
            IUnitOfWorkFactory uowFactory,
            IOptions<TaskQueueSettings> configuration,
            ILogger<TaskQueueService> logger,
            IWebApiClient client,
            IEjecucionService ejecucionService,
            IEmpresaService empresaService,
            IParameterService parameterService,
            IWebhookCallerService webhookCallerService,
            IReportGeneratorService reportGeneratorService)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
            _uowFactory = uowFactory;
            _ejecucionService = ejecucionService;
            _empresaService = empresaService;
            _parameterService = parameterService;
            _webhookCallerService = webhookCallerService;
            _reportGeneratorService = reportGeneratorService;
        }

        public virtual List<Dictionary<string, string>> LoadTasks(string category)
        {
            switch (category)
            {
                case TaskQueueCategory.WEBHOOK:
                    return GetWebhookTasks();
                case TaskQueueCategory.API:
                    return GetAPITasks();
                case TaskQueueCategory.REPORT:
                    return GetReportTasks();
                default:
                    return new List<Dictionary<string, string>>();
            }
        }

        public virtual async Task Init()
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int.TryParse(await uow.ParametroRepository.GetParametro(ParamManager.WEBHOOK_TIMEOUT), out _timeout);
                int.TryParse(await uow.ParametroRepository.GetParametro(ParamManager.WEBHOOK_ESPERA_REINTENTO), out _retrySleep);
                int.TryParse(await uow.ParametroRepository.GetParametro(ParamManager.WEBHOOK_REINTENTOS), out _maxRetries);

                _isOnDemandReportProcessing = await uow.ParametroRepository.GetParametro(ParamManager.REPORTE_ON_DEMAND) == "S";
                _retrySleep = _retrySleep * 1000;
            }
        }

        public virtual async Task Process(string category, Dictionary<string, string> data)
        {
            await Queue.DispatchAsync<Task>(() =>
            {
                switch (category)
                {
                    case TaskQueueCategory.WEBHOOK:
                        return ProcessWebhookTask(data);
                    case TaskQueueCategory.API:
                        return ProcessAPITask(data);
                    case TaskQueueCategory.REPORT:
                        return ProcessReportTask(data);
                    default:
                        return null;
                }
            });
        }

        public virtual bool IsEnabled()
        {
            return _configuration.Value.IsEnabled;
        }

        public virtual bool IsOnDemandReportProcessing()
        {
            if (!_isOnDemandReportProcessing.HasValue)
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    _isOnDemandReportProcessing = uow.ParametroRepository.GetParametro(ParamManager.REPORTE_ON_DEMAND).GetAwaiter().GetResult() == "S";
                }
            }

            return _isOnDemandReportProcessing.Value;
        }

        public virtual void Enqueue(string category, int interfazExterna, string key)
        {
            Enqueue(category, interfazExterna, new List<string>() { key });
        }

        public virtual void Enqueue(string category, int interfazExterna, List<string> keys)
        {
            Enqueue(category, keys, new Dictionary<string, string>
            {
                { "Interfaz", interfazExterna.ToString() },
            });
        }

        public virtual void Enqueue(string category, List<string> keys)
        {
            Enqueue(category, keys, new Dictionary<string, string>());
        }

        public virtual void Enqueue(string category, List<string> keys, Dictionary<string, string> extraData)
        {
            try
            {
                var payload = new TasksRequest();
                foreach (var key in keys.Where(x => !string.IsNullOrEmpty(x)))
                {
                    var request = new TaskRequest()
                    {
                        Category = category,
                        Data = new Dictionary<string, string>()
                        {
                            { "Key", key }
                        }
                    };

                    foreach (var extraDataKey in extraData.Keys)
                    {
                        request.Data[extraDataKey] = extraData[extraDataKey];
                    }

                    payload.Tasks.Add(request);
                }

                if (payload.Tasks.Count > 0)
                {
                    HttpResponseMessage response = this._client.Post(new Uri(new Uri(this._configuration.Value.Endpoint), "/TaskQueue/Enqueue"), payload);

                    if (!response.IsSuccessStatusCode)
                        _logger.LogError($"Error al comunicarse con el API TaskQueue/Enqueue. Status: {response.StatusCode}. Message: {response.ReasonPhrase}. Datos: {payload} ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al comunicarse con el API TaskQueue/Enqueue. ExtraData: {extraData} - Keys: {keys} - Exception{ex}");
            }
        }

        public virtual void Restart()
        {
            try
            {
                HttpResponseMessage response = this._client.Post<string>(new Uri(new Uri(this._configuration.Value.Endpoint), "/TaskQueue/Restart"), string.Empty);

                if (!response.IsSuccessStatusCode)
                    _logger.LogError($"Error al comunicarse con la api TaskQueue/Restart. Status: {response.StatusCode}. Message: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al comunicarse con la api TaskQueue/Restart. Interfaz: - Exception{ex}");
            }
        }

        #region Load

        public virtual List<Dictionary<string, string>> GetWebhookTasks()
        {
            var tasks = new List<Dictionary<string, string>>();
            var task = _ejecucionService.GetNotificacionesPendientes();

            task.Wait();

            foreach (var interfaz in task.Result)
            {
                tasks.Add(new Dictionary<string, string>()
                {
                    { "Interfaz", interfaz.CdInterfazExterna.Value.ToString() },
                    { "Key", interfaz.Id.ToString() }
                });
            }

            return tasks;
        }

        public virtual List<Dictionary<string, string>> GetAPITasks()
        {
            var tasks = new List<Dictionary<string, string>>();
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                tasks.AddRange(GetAjustesPendientes(uow));
                tasks.AddRange(GetPedidosAnuladosPendientes(uow));
                tasks.AddRange(GetCamionesPendientesDeFacturacion(uow));
                tasks.AddRange(GetAgendasPendientesDeConfirmacion(uow));
                tasks.AddRange(GetAlmacenamientosPendientesDeConfirmacion(uow));
                tasks.AddRange(GetCamionesPendientesConfirmarCierre(uow));
                tasks.AddRange(GetProduccionesPendientesConfirmacion(uow));
            }
            return tasks.OrderBy(t => DateTime.Parse(t["Fecha"], CultureInfo.InvariantCulture))
                .ThenBy(t => t["InterfaceOrder"]).ThenBy(t => t["KeyOrder"]).ToList();
        }

        public virtual List<Dictionary<string, string>> GetReportTasks()
        {
            var tasks = new List<Dictionary<string, string>>();
            var pendingReports = _reportGeneratorService.GetPendingReports();

            foreach (var reportId in pendingReports)
            {
                tasks.Add(new Dictionary<string, string>()
                {
                    { "Key", reportId.ToString() }
                });
            }

            return tasks;
        }

        public virtual List<Dictionary<string, string>> GetAjustesPendientes(IUnitOfWork uow)
        {
            var data = new List<Dictionary<string, string>>();
            var task = uow.AjusteRepository.GetAjustesPendientesDeConfirmacion();

            task.Wait();

            foreach (var t in task.Result)
            {
                data.Add(new Dictionary<string, string>()
                    {
                        { "Interfaz", CInterfazExterna.AjustesDeStock.ToString()},
                        { "Key", t.Id }, //NU_AJUSTE_STOCK
                        { "Fecha", t.Fecha.ToString(CultureInfo.InvariantCulture) }, //DT_REALIZADO
                        { "InterfaceOrder", "1" },
                        { "KeyOrder", CompletarKey(t.Id, 10) }
                    });
            }
            return data;
        }
        public virtual List<Dictionary<string, string>> GetPedidosAnuladosPendientes(IUnitOfWork uow)
        {
            var data = new List<Dictionary<string, string>>();
            var task = uow.PedidoRepository.GetPedidosAnuladosPendientes();

            task.Wait();

            foreach (var t in task.Result)
            {
                data.Add(new Dictionary<string, string>()
                    {
                        { "Interfaz", CInterfazExterna.PedidosAnulados.ToString()},
                        { "Key", t.Id }, //NU_PEDIDO#CD_CLIENTE#CD_EMPRESA
                        { "Fecha", t.Fecha.ToString(CultureInfo.InvariantCulture) }, //DT_ADDROW
                        { "InterfaceOrder", "2" },
                        { "KeyOrder", CompletarKey(t.Id, 15) }
                    });
            }

            return data;
        }
        public virtual List<Dictionary<string, string>> GetAgendasPendientesDeConfirmacion(IUnitOfWork uow)
        {
            var data = new List<Dictionary<string, string>>();
            var task = uow.AgendaRepository.GetAgendasPendientesDeConfirmacion();

            task.Wait();

            foreach (var t in task.Result)
            {
                data.Add(new Dictionary<string, string>()
                    {
                        { "Interfaz", CInterfazExterna.ConfirmacionDeRecepcion.ToString()},
                        { "Key", t.Id }, //NU_AGENDA
                        { "Fecha", t.Fecha.ToString(CultureInfo.InvariantCulture) }, //DT_CIERRE
                        { "InterfaceOrder", "3" },
                        { "KeyOrder", CompletarKey(t.Id, 8) }
                    });
            }

            return data;
        }
        public virtual List<Dictionary<string, string>> GetAlmacenamientosPendientesDeConfirmacion(IUnitOfWork uow)
        {
            var data = new List<Dictionary<string, string>>();
            var task = uow.EtiquetaLoteRepository.GetAlmacenamientosPendientesDeConfirmacion();

            task.Wait();

            foreach (var t in task.Result)
            {
                data.Add(new Dictionary<string, string>()
                {
                    { "Interfaz", CInterfazExterna.Almacenamiento.ToString()},
                    { "Key", t.Id }, //NU_AGENDA#NU_ETIQUETA_LOTE
                    { "Fecha", t.Fecha.ToString(CultureInfo.InvariantCulture) }, //DT_OPERACION
                    { "InterfaceOrder", "4" },
                    { "KeyOrder", CompletarKey(t.Id, 10) }
                });
            }

            return data;
        }
        public virtual List<Dictionary<string, string>> GetCamionesPendientesDeFacturacion(IUnitOfWork uow)
        {
            var data = new List<Dictionary<string, string>>();
            var task = uow.CamionRepository.GetCamionesPendientesDeFacturacion();

            task.Wait();

            foreach (var t in task.Result)
            {
                data.Add(new Dictionary<string, string>()
                    {
                        { "Interfaz", CInterfazExterna.Facturacion.ToString()},
                        { "Key", t.Id }, //CD_CAMION
                        { "Fecha", t.Fecha.ToString(CultureInfo.InvariantCulture) }, //DT_FACTURACION
                        { "InterfaceOrder", "5" },
                        { "KeyOrder", CompletarKey(t.Id, 10) }
                    });
            }

            return data;
        }
        public virtual List<Dictionary<string, string>> GetCamionesPendientesConfirmarCierre(IUnitOfWork uow)
        {
            var data = new List<Dictionary<string, string>>();
            var task = uow.CamionRepository.GetCamionesPendientesConfirmarCierre();

            task.Wait();

            foreach (var t in task.Result)
            {
                data.Add(new Dictionary<string, string>()
                    {
                        { "Interfaz", CInterfazExterna.ConfirmacionDePedido.ToString()},
                        { "Key", t.Id }, //CD_CAMION
                        { "Fecha", t.Fecha.ToString(CultureInfo.InvariantCulture) }, //DT_CIERRE
                        { "InterfaceOrder", "6" },
                        { "KeyOrder", CompletarKey(t.Id, 10) }
                    });
            }

            return data;
        }
        public virtual List<Dictionary<string, string>> GetProduccionesPendientesConfirmacion(IUnitOfWork uow)
        {
            var data = new List<Dictionary<string, string>>();
            var task = uow.IngresoProduccionRepository.GetProduccionesPendientesDeNotificar();

            task.Wait();

            foreach (var t in task.Result)
            {
                data.Add(new Dictionary<string, string>()
                {
                    { "Interfaz", CInterfazExterna.ConfirmacionProduccion.ToString()},
                    { "Key", t.Id }, //NU_PRDC_INGRESO
                    { "Fecha", t.Fecha.ToString(CultureInfo.InvariantCulture) },
                    { "InterfaceOrder", "7" },
                    { "KeyOrder", CompletarKey(t.Id, 10) }
                });
            }

            return data;
        }

        public virtual string CompletarKey(string value, int largo)
        {
            if (value.Length < largo)
                return value.PadLeft(largo, '0');
            return value;
        }

        #endregion

        #region Process

        public virtual async Task ProcessWebhookTask(Dictionary<string, string> data)
        {
            var nroEjecucion = long.Parse(data["Key"]);
            var interfaz = await _ejecucionService.GetEjecucion(nroEjecucion);
            await NotifyWebhookIfConfigured(interfaz);
        }

        public virtual async Task NotifyWebhookIfConfigured(InterfazEjecucion interfaz)
        {
            await Queue.DispatchSync<Task>(async () =>
            {
                var interfazHabilitada = false;
                var interfazExterna = interfaz.CdInterfazExterna.Value;
                var empresa = await _empresaService.GetEmpresa(interfaz.Empresa.Value);

                if (interfaz.InterfazExterna != null && !string.IsNullOrEmpty(interfaz.InterfazExterna.ParametroDeHabilitacion))
                {
                    interfazHabilitada = (_parameterService.GetValueByEmpresa(interfaz.InterfazExterna.ParametroDeHabilitacion, empresa.Id) ?? "N") == "S";
                }

                if (!interfazHabilitada)
                {
                    interfaz.Situacion = SituacionDb.ProcesadoOK;
                    await _ejecucionService.UpdateEjecucion(interfaz);
                }
                else if (empresa.IsNotifiedByWebhook && !empresa.IsLocked)
                {
                    var camelCaseEnabled = IsCamelCaseEnabled(empresa.Id);
                    interfaz = await _ejecucionService.GetEjecucion(interfaz.Id);

                    if (interfaz.Situacion == SituacionDb.ProcesadoPendiente)
                    {
                        var itfzData = await _ejecucionService.GetEjecucionData(interfaz.Id);
                        _logger.LogDebug($"Notificando interfaz {interfaz.Id} por Webhook");
                        var error = await TryNotifyWebhook(empresa, interfaz.Id, interfazExterna, itfzData.Data, _timeout, camelCaseEnabled);
                        var retries = 0;

                        while (error != null && retries < _maxRetries)
                        {
                            Thread.Sleep(_retrySleep);
                            error = await TryNotifyWebhook(empresa, interfaz.Id, interfazExterna, itfzData.Data, _timeout, camelCaseEnabled);
                            retries++;
                        }

                        if (error != null)
                        {
                            await ProcessWebhookError(interfaz, empresa, error);
                        }
                        else
                        {
                            interfaz.Situacion = SituacionDb.ProcesadoOK;
                            await _ejecucionService.UpdateEjecucion(interfaz);
                        }
                    }
                }
            });
        }

        public virtual async Task ProcessAPITask(Dictionary<string, string> data)
        {
            var nroEjecuciones = await GenerarInterfaces(data);

            foreach (var nroEjecucion in nroEjecuciones)
            {
                var interfaz = await _ejecucionService.GetEjecucion(nroEjecucion);

                if (interfaz.Situacion == SituacionDb.ProcesadoPendiente)
                    await NotifyWebhookIfConfigured(interfaz);
            }
        }

        public virtual async Task ProcessReportTask(Dictionary<string, string> data)
        {
            var nroReporte = long.Parse(data["Key"]);
            await _reportGeneratorService.GeneratePendingReport(nroReporte);
        }

        public virtual async Task<List<long>> GenerarInterfaces(Dictionary<string, string> data)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var interfaz = int.Parse(data["Interfaz"]);
                var key = data["Key"];
                var GetGC = new Func<int?, string>(e => GetGrupoConsulta(uow, interfaz, key, e));

                switch (interfaz)
                {
                    case CInterfazExterna.AjustesDeStock:
                        return await uow.AjusteRepository.GenerarInterfaces(int.Parse(key), GetGC);
                    case CInterfazExterna.PedidosAnulados:
                        return await uow.PedidoRepository.GenerarInterfaces(key, GetGC);
                    case CInterfazExterna.Facturacion:
                        return await uow.CamionRepository.GenerarInterfacesFacturacion(int.Parse(key), GetGC);
                    case CInterfazExterna.ConfirmacionDeRecepcion:
                        return await uow.AgendaRepository.GenerarInterfaces(int.Parse(key), GetGC);
                    case CInterfazExterna.Almacenamiento:
                        return await uow.EtiquetaLoteRepository.GenerarInterfaces(key, GetGC);
                    case CInterfazExterna.ConfirmacionDePedido:
                        return await uow.CamionRepository.GenerarInterfacesCierre(int.Parse(key), GetGC);
                    case CInterfazExterna.ConfirmacionProduccion:
                        return await uow.IngresoProduccionRepository.GenerarInterfacesConfirmacionProduccion(key, GetGC);
                    default:
                        return new List<long>();
                }
            }
        }

        public virtual string GetGrupoConsulta(IUnitOfWork uow, int interfazExterna, string key, int? empresa)
        {
            var entidadParam = empresa.HasValue ? ParamManager.PARAM_EMPR : ParamManager.PARAM_GRAL;
            return uow.ParametroRepository.GetParametro(ParamManager.GRUPO_CONSULTA, entidadParam, empresa?.ToString()).Result ?? "S/N";
        }

        public virtual async Task<Exception> TryNotifyWebhook(Empresa empresa, long nuInterfazEjecucion, int interfazExterna, byte[] data, int timeout, bool camelCaseEnabled)
        {
            try
            {
                _logger.LogDebug($"TryNotifyWebhook - Inicio - Nro. Ejecución: {nuInterfazEjecucion}");

                var dataContent = Encoding.UTF8.GetString(data);

                JsonSerializerOptions serializeOptions = null;
                if (camelCaseEnabled)
                {
                    serializeOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };
                }

                var content = JsonSerializer.Serialize(WebhookEvent.GetNewInstance(nuInterfazEjecucion, interfazExterna, dataContent, camelCaseEnabled), serializeOptions);
                var payloadUrl = empresa.PayloadUrl;
                var hash = _empresaService.GetFirma(empresa.Id, content);
                _logger.LogDebug("TryNotifyWebhook - PostGetFirma");

                LogWebHookNotificationContent(nuInterfazEjecucion, content);

                await _webhookCallerService.Invoke(payloadUrl, hash, content, timeout);

                _logger.LogDebug($"TryNotifyWebhook - Fin - Nro. Ejecución: {nuInterfazEjecucion}");

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"TryNotifyWebhook - Nro. Interfaz Ejecucion {nuInterfazEjecucion} - Error: {ex.Message}");
                return ex;
            }
        }

        public virtual void LogWebHookNotificationContent(long nuInterfazEjecucion, string content)
        {
            _logger.LogDebug($"TryNotifyWebhook - Nro. Ejecución: {nuInterfazEjecucion} - Content: {content}");

            var path = string.IsNullOrEmpty(_configuration.Value.WebHookNotificactionLogPath) ? "C:\\WIS\\WebHookNotifications" : _configuration.Value.WebHookNotificactionLogPath;

            if (!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var filename = $"{nuInterfazEjecucion}.json";
            if (!Path.Exists(Path.Combine(path, filename)))
            {
                using (var writer = new StreamWriter(Path.Combine(path, filename)))
                {
                    writer.Write(content);
                }
            }
        }

        public virtual async Task ProcessWebhookError(InterfazEjecucion interfaz, Empresa empresa, Exception error)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var evento = uow.EventoRepository.GetEvento(EventoDb.WebhookError);

                interfaz.Situacion = SituacionDb.ProcesadoConError;
                interfaz.ErrorProcedimiento = "S";

                await _ejecucionService.AddError(interfaz, 0, error.Message);
                await _ejecucionService.UpdateEjecucion(interfaz);
                await _empresaService.UpdateLock(empresa.Id, true);

                if (evento != null)
                {
                    var instancias = uow.EventoRepository.GetInstanciasHabilitadas(evento.Id, TipoNotificacionDb.Email, EventoParametroDb.CD_EMPRESA, empresa.Id.ToString());

                    foreach (var instancia in instancias)
                    {
                        AddWebhookErrorNotifications(uow, instancia, empresa, interfaz.Id);
                    }

                    uow.SaveChanges();
                }
            }

            throw error;
        }

        public virtual void AddWebhookErrorNotifications(IUnitOfWork uow, Instancia instancia, Empresa empresa, long nroEjecucion)
        {
            var contactos = uow.DestinatarioRepository.GetContactos(instancia.Id);
            var asunto = instancia.Template?.Asunto ?? "Error al Notificar Salida";
            var cuerpo = instancia.Template?.Cuerpo ?? "Ocurrió un error al notificar salida {interfaz}.";

            cuerpo = Smart.Format(cuerpo, GetParametrosPlantillaNotificacion(uow, nroEjecucion, empresa));

            foreach (var contacto in contactos)
            {
                if (!string.IsNullOrEmpty(contacto.Email))
                {
                    uow.NotificacionRepository.AddNotificacionEmail(new NotificacionEmail()
                    {
                        Asunto = asunto,
                        Cuerpo = cuerpo,
                        EmailRecibe = contacto.Email,
                        Estado = EstadoNotificacion.EST_PEND,
                        FechaEnvio = DateTime.Now,
                        NumeroInstancia = instancia.Id,
                        IsHtml = instancia.Template?.IsHtml,
                        Archivos = new List<NotificacionArchivo>()
                        {
                            new NotificacionArchivo()
                            {
                                IdReferencia = nroEjecucion.ToString(),
                                TpReferencia = EventoArchivoTipoReferenciaDb.INTERFAZ,
                                DsArchivo = $"{nroEjecucion}.json",
                                DtAddRow = DateTime.Now,
                                DtUpdateRow = DateTime.Now,
                            }
                        }
                    });
                }
            }
        }

        public virtual object GetParametrosPlantillaNotificacion(IUnitOfWork uow, long nroEjecucion, Empresa empresa)
        {
            return new
            {
                interfaz = nroEjecucion,
                empresa = empresa.Nombre + $" [{empresa.Id}]",
                reintentos = _maxRetries,
                destino = empresa.PayloadUrl
            };
        }

        public virtual bool IsCamelCaseEnabled(int empresa)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return uow.ParametroRepository.GetParametro(ParamManager.WEBHOOK_CAMELCASE_ENABLED, ParamManager.PARAM_EMPR, empresa.ToString()).Result == "S";
            }
        }
        #endregion
    }
}
