using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using WIS.Configuration;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence;
using WIS.Persistence.Database;
using WIS.XmlProcessorEntrada.Helpers;
using WIS.XmlProcessorEntrada.Models;

namespace WIS.XmlProcessorEntrada.Services
{
    public class XmlDataExternalManager : IXmlDataExternalManager
    {
        protected ApiMapper _mapper;
        protected ExtraDataProcessor _extraDataProcessor;
        protected readonly IConfiguration _configuration;
        protected readonly ILogger<XmlDataExternalManager> _logger;
        protected readonly IOptions<DatabaseSettings> _dbSettings;
        protected readonly IDatabaseConfigurationService _dbConfigService;

        protected List<int> CodigosInterfazEntrada { get; set; }

        public XmlDataExternalManager(
            IConfiguration configuration,
            ILogger<XmlDataExternalManager> logger,
            IOptions<DatabaseSettings> dbSettings,
            IDatabaseConfigurationService dbConfigService)
        {
            _configuration = configuration;
            _logger = logger;
            _dbSettings = dbSettings;
            _dbConfigService = dbConfigService;

            Initialize();
        }

        public virtual async Task Start()
        {
            try
            {
                var wmsApiEndpoint = _configuration.GetValue<string>("WmsApiSettings:Endpoint");
                var clientId = _configuration.GetValue<string>("AuthSettings:ClientId");
                var clientSecret = _configuration.GetValue<string>("AuthSettings:ClientSecret");
                var address = _configuration.GetValue<string>("AuthSettings:TokenUrl");
                var scope = _configuration.GetValue<string>("AuthSettings:AccessScope");
                var token = await GetTokenAsync(address, clientId, clientSecret, scope);

                using (WISDB context = GetNewDbContext())
                {
                    var userId = GetUserId(context, clientId);
                    var interfaces_habilitadas = GetInterfacesHabilitadas(context, userId);
                    var pending_ejecs = GetEjecucionesPendientesEntrada(context, userId);

                    foreach (var ejec in pending_ejecs)
                    {
                        var empresa = ejec.CD_EMPRESA.Value;
                        var interfazExterna = ejec.CD_INTERFAZ_EXTERNA.Value;

                        _logger.LogDebug($"Procesando la interfaz {ejec.NU_INTERFAZ_EJECUCION}.");

                        if (InterfazHabilitada(interfaces_habilitadas, empresa, interfazExterna))
                        {
                            var ejec_data = ejec.T_INTERFAZ_EJECUCION_DATA;
                            var extraData = (object)null;

                            if (ejec_data != null)
                            {
                                var xml = ByteString.GetString(ejec_data.DATA);
                                var omitirInterfazActual = false;
                                var omitirApis = false;
                                var firstNonProcessedRequest = 0;
                                var requests = new List<ApiRequest>();

                                try
                                {
                                    requests.AddRange(_mapper.Map(interfazExterna, empresa, ejec_data.NU_INTERFAZ_EJECUCION, xml, out extraData));
                                }
                                catch (Exception ex)
                                {
                                    requests = new List<ApiRequest>();
                                 
                                    ejec.FL_ERROR_CARGA = "S";
                                    ejec.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                                    ejec.DT_SITUACAO = DateTime.Now;

                                    ErrorGenericoInterfaz(context, ejec, ex.Message, cdError: ProcessorCDError.WIE_104);

                                    omitirInterfazActual = true;
                                }

                                foreach (var request in requests)
                                {
                                    if (interfazExterna == CInterfazExterna.Empresas && request.InterfazExterna == CInterfazExterna.Agentes)
                                    {
                                        continue;
                                    }

                                    if (!InterfazHabilitada(interfaces_habilitadas, request.Empresa, request.InterfazExterna))
                                    {
                                        ejec.FL_ERROR_CARGA = "S";
                                        ejec.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                                        ejec.DT_SITUACAO = DateTime.Now;

                                        ErrorGenericoInterfaz(context, ejec, $"Interfaz {request.InterfazExterna} no habilitada para la empresa {ejec.CD_EMPRESA}");

                                        omitirInterfazActual = true;

                                        break;
                                    }
                                }

                                if (omitirInterfazActual)
                                {
                                    context.SaveChanges();
                                    continue;
                                }

                                var jsonIdRequestPrefix = string.Format(ApiMapper.WISIE_ID_REQUEST_FORMAT, ejec.NU_INTERFAZ_EJECUCION, "");
                                var jsonInterfaces = context.T_INTERFAZ_EJECUCION
                                    .Include("T_INTERFAZ_EJECUCION_ERROR")
                                    .AsNoTracking()
                                    .Where(ie => ie.CD_EMPRESA == empresa && ie.ID_REQUEST.StartsWith(jsonIdRequestPrefix))
                                    .ToList();

                                if (jsonInterfaces.Count > 0)
                                {
                                    var anyError = jsonInterfaces.Any(i => i.CD_SITUACAO == SituacionDb.ProcesadoConError);
                                    var anyPending = jsonInterfaces.Count < requests.Count;

                                    if (!anyError && !anyPending)
                                    {
                                        omitirApis = true;
                                        omitirInterfazActual = false;                                        
                                    }
                                    else if (anyError)
                                    {
                                        var apiError = jsonInterfaces.First(i => i.CD_SITUACAO == SituacionDb.ProcesadoConError);
                                        var firstError = apiError.T_INTERFAZ_EJECUCION_ERROR.FirstOrDefault();
                                        var errDetail = firstError?.DS_ERROR ?? "";
                                        var errMessage = $"Error al invocar al API {apiError.CD_INTERFAZ_EXTERNA} mientras se procesaba la interfaz {ejec.NU_INTERFAZ_EJECUCION}. Detalle: {errDetail}.";

                                        ejec.FL_ERROR_CARGA = apiError.FL_ERROR_CARGA;
                                        ejec.FL_ERROR_PROCEDIMIENTO = apiError.FL_ERROR_PROCEDIMIENTO;
                                        ejec.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                                        ejec.DT_SITUACAO = DateTime.Now;

                                        ErrorGenericoInterfaz(context, ejec, errMessage);

                                        omitirApis = true;
                                        omitirInterfazActual = true;
                                    }
                                    else 
                                    {
                                        firstNonProcessedRequest = jsonInterfaces.Count;
                                    }
                                }

                                if (!omitirApis)
                                {
                                    for (int i = firstNonProcessedRequest; i < requests.Count; i++)
                                    {
                                        var request = requests[i];
                                        var existeJson = context.T_INTERFAZ_EJECUCION
                                            .AsNoTracking()
                                            .Any(ie => ie.CD_EMPRESA == empresa && ie.ID_REQUEST == request.IdRequest);

                                        if (existeJson)
                                        {
                                            //No debería darse este caso, ya que si la interfaz JSON no fue procesada no debería existir una interfaz JSON con su IdRequest
                                            //Se agrega igualmente la validación como control preventivo
                                            continue;
                                        }

                                        try
                                        {
                                            if (interfazExterna == CInterfazExterna.Empresas && request.InterfazExterna == CInterfazExterna.Agentes)
                                            {
                                                HabilitarInterfaz(context, userId, request.Empresa, request.InterfazExterna);
                                            }

                                            var action = GetAction(request);
                                            var uriString = wmsApiEndpoint + action;

                                            _logger.LogDebug($"Invocando al API {request.InterfazExterna} como parte del procesamiento de la interfaz {ejec.NU_INTERFAZ_EJECUCION}.");

                                            var response = await InvokeAsync<ApiResponse>(uriString, HttpMethod.Post, token, request.Payload);

                                            _logger.LogDebug($"Interfaz {response.NumeroInterfaz} generada por el API {request.InterfazExterna} mientras se procesaba la interfaz {ejec.NU_INTERFAZ_EJECUCION}.");
                                        }
                                        catch (ApiResponseException ex)
                                        {
                                            var problemDetails = ex.ProblemDetails;
                                            var errMessage = $"Error al invocar al API {request.InterfazExterna} mientras se procesaba la interfaz {ejec.NU_INTERFAZ_EJECUCION}. Detalle: {problemDetails.Detail}";
                                            bool? errorCarga = GetErrorCarga(context, problemDetails);

                                            ejec.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                                            ejec.DT_SITUACAO = DateTime.Now;

                                            if (errorCarga ?? true)
                                            {
                                                ejec.FL_ERROR_CARGA = "S";
                                                ejec.FL_ERROR_PROCEDIMIENTO = "N";
                                            }
                                            else
                                            {
                                                ejec.FL_ERROR_CARGA = "N";
                                                ejec.FL_ERROR_PROCEDIMIENTO = "S";
                                            }

                                            _logger.LogError(ex, errMessage);

                                            ErrorGenericoInterfaz(context, ejec, errMessage);

                                            omitirInterfazActual = true;

                                            break;
                                        }
                                        catch (Exception ex)
                                        {
                                            var errMessage = $"Error al invocar al API {request.InterfazExterna} mientras se procesaba la interfaz {ejec.NU_INTERFAZ_EJECUCION}. Detalle: {ex.Message}.";

                                            ejec.FL_ERROR_CARGA = "S";
                                            ejec.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                                            ejec.DT_SITUACAO = DateTime.Now;

                                            _logger.LogError(ex, errMessage);

                                            ErrorGenericoInterfaz(context, ejec, errMessage);

                                            omitirInterfazActual = true;

                                            break;
                                        }
                                    }
                                }

                                if (omitirInterfazActual)
                                {
                                    context.SaveChanges();
                                    continue;
                                }

                                if (extraData != null)
                                {
                                    _extraDataProcessor.ProcessExtraData(context, extraData);
                                }

                                ejec.FL_ERROR_CARGA = "N";
                                ejec.FL_ERROR_PROCEDIMIENTO = "N";
                                ejec.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                                ejec.DT_SITUACAO = DateTime.Now;
                            }
                            else
                            {
                                ejec.FL_ERROR_CARGA = "S";
                                ejec.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                                ejec.DT_SITUACAO = DateTime.Now;

                                ErrorInterfazSinDatos(context, ejec);
                            }
                        }
                        else
                        {
                            ejec.FL_ERROR_CARGA = "S";
                            ejec.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                            ejec.DT_SITUACAO = DateTime.Now;

                            ErrorGenericoInterfaz(context, ejec, $"Interfaz {ejec.CD_INTERFAZ_EXTERNA} no habilitada para la empresa {ejec.CD_EMPRESA}");
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "XmlDataExternalManager.Start");
            }
        }

        protected virtual void Initialize()
        {
            _mapper = new ApiMapper();
            _extraDataProcessor = new ExtraDataProcessor();

            CodigosInterfazEntrada = new List<int>()
            {
                CInterfazExterna.Empresas,
                CInterfazExterna.Producto,
                CInterfazExterna.ProductoProveedor,
                CInterfazExterna.CodigoDeBarras,
                CInterfazExterna.Agentes,
                CInterfazExterna.ReferenciaDeRecepcion,
                CInterfazExterna.ModificarDetalleReferenciaRecepcion,
                CInterfazExterna.AnulacionReferenciaRecepcion,
                CInterfazExterna.Pedidos,
                CInterfazExterna.ModificarPedido,
            };
        }

        protected virtual WISDB GetNewDbContext()
        {
            return new WISDB(_dbConfigService, _dbSettings.Value.ConnectionString, _dbSettings.Value.Schema);
        }

        protected virtual List<T_INTERFAZ_EJECUCION> GetEjecucionesPendientesEntrada(WISDB context, int userId)
        {
            return context.T_INTERFAZ_EJECUCION
                .Join(context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == userId),
                    ie => ie.CD_EMPRESA,
                    ef => ef.CD_EMPRESA,
                    (ie, ef) => ie)
                .Include("T_INTERFAZ_EJECUCION_DATA")
                .Where(x => CodigosInterfazEntrada.Contains((int)x.CD_INTERFAZ_EXTERNA)
                    && x.CD_SITUACAO == SituacionDb.ArchivoProcesado)
                .OrderBy(x => x.NU_INTERFAZ_EJECUCION)
                .ToList();
        }

        protected virtual Dictionary<int, HashSet<int>> GetInterfacesHabilitadas(WISDB context, int userId)
        {
            var result = new Dictionary<int, HashSet<int>>();

            var parametros = CodigosInterfazEntrada
                .Select(c => $"IE_{c}_HABILITADA");

            var entidades = context.T_EMPRESA_FUNCIONARIO
                .AsNoTracking()
                .Where(ef => ef.USERID == userId)
                .Select(ef => $"{ParamManager.PARAM_EMPR}_{ef.CD_EMPRESA}")
                .ToList();

            var configuraciones = context.T_LPARAMETRO_CONFIGURACION
                .AsNoTracking()
                .Where(c => parametros.Contains(c.CD_PARAMETRO)
                    && c.DO_ENTIDAD_PARAMETRIZABLE == ParamManager.PARAM_EMPR
                    && c.VL_PARAMETRO == "S")
                .ToList();

			configuraciones = configuraciones
			   .Where(c => entidades.Contains(c.ND_ENTIDAD))
			   .ToList();

			foreach (var c in configuraciones)
            {
                var empresa = int.Parse(c.ND_ENTIDAD.Replace($"{ParamManager.PARAM_EMPR}_", ""));
                var interfaz = int.Parse(c.CD_PARAMETRO.Replace("IE_", "").Replace("_HABILITADA", ""));

                if (!result.ContainsKey(empresa))
                {
                    result[empresa] = new HashSet<int>();
                }

                if (!result[empresa].Contains(interfaz))
                {
                    result[empresa].Add(interfaz);
                }
            }

            return result;
        }

        protected virtual int GetUserId(WISDB context, string clientId)
        {
            return context.USERS.FirstOrDefault(u => u.LOGINNAME.ToLower().Trim() == clientId.ToLower().Trim()).USERID;
        }

        protected virtual void HabilitarInterfaz(WISDB context, int userId, int empresa, int interfazExterna)
        {
            var cdParametro = $"IE_{interfazExterna}_HABILITADA";
            var doEntidadParametrizable = ParamManager.PARAM_EMPR;
            var ndEntidad = $"{ParamManager.PARAM_EMPR}_{empresa}";
            var existeParametro = context.T_LPARAMETRO_CONFIGURACION
                .AsNoTracking()
                .Any(pc => pc.CD_PARAMETRO == cdParametro
                    && pc.DO_ENTIDAD_PARAMETRIZABLE == doEntidadParametrizable
                    && pc.ND_ENTIDAD == ndEntidad);

            if (!existeParametro)
            {
                context.T_LPARAMETRO_CONFIGURACION.Add(new T_LPARAMETRO_CONFIGURACION
                {
                    CD_PARAMETRO = cdParametro,
                    DO_ENTIDAD_PARAMETRIZABLE = doEntidadParametrizable,
                    ND_ENTIDAD = ndEntidad,
                    VL_PARAMETRO = "S",
                });
            }

            var empresaAsignada = context.T_EMPRESA_FUNCIONARIO
                .AsNoTracking()
                .Any(ef => ef.CD_EMPRESA == empresa && ef.USERID == userId);

            if (!empresaAsignada)
            {
                context.T_EMPRESA_FUNCIONARIO.Add(new T_EMPRESA_FUNCIONARIO
                { 
                    CD_EMPRESA = empresa,
                    USERID = userId,
                });
            }

			context.SaveChanges();
		}

        protected virtual bool InterfazHabilitada(Dictionary<int, HashSet<int>> interfaceHabilitadas, int empresa, int interfazExterna)
        {
            return interfaceHabilitadas.ContainsKey(empresa) && interfaceHabilitadas[empresa].Contains(interfazExterna);
        }

        public virtual void ErrorInterfazSinDatos(WISDB context, T_INTERFAZ_EJECUCION ejec)
        {
            var errMessage = $"No hay datos para procesar asociados a la ejecución {ejec.NU_INTERFAZ_EJECUCION}";
            var errors = new List<string> { errMessage };

            _logger.LogDebug(errMessage);

            XmlProcessorError.SaveErrores(context, ejec.NU_INTERFAZ_EJECUCION, errors, ProcessorCDError.WIE_101);
        }

        protected virtual int ErrorGenericoInterfaz(WISDB context, T_INTERFAZ_EJECUCION ejec, string errMessage, int? nuError = null, ProcessorCDError cdError = ProcessorCDError.WIE_100)
        {
            var errors = new List<string> { errMessage };

            _logger.LogDebug(errMessage);

            return XmlProcessorError.SaveErrores(context, ejec.NU_INTERFAZ_EJECUCION, errors, cdError, nuError);
        }

        protected static async Task<string> GetTokenAsync(string address, string clientId, string clientSecret, string scope)
        {
            using (var client = new HttpClient())
            {
                var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = address,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = scope
                });

                if (response.IsError)
                    throw new Exception($"An error occurred while retrieving an access token: {response.Error}");

                return response.AccessToken;
            }
        }

        protected virtual async Task<T> InvokeAsync<T>(string uriString, HttpMethod method, string token, object request)
        {
            var uriBuilder = new UriBuilder(uriString);
            var jsonContent = string.Empty;
            var jsonSerializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            if (method == HttpMethod.Get)
            {
                if (request != null)
                {
                    var properties = from p in request.GetType().GetProperties()
                                     where p.GetValue(request, null) != null
                                     select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(request, null)?.ToString());

                    uriBuilder.Query = string.Join("&", properties);
                }
            }
            else
            {
                jsonContent = JsonSerializer.Serialize(request);
            }

            var requestUri = uriBuilder.Uri;

            using (var client = new HttpClient())
            using (var requestMessage = new HttpRequestMessage(method, requestUri))
            {
                client.Timeout = TimeSpan.FromMinutes(30);

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                requestMessage.Headers.ConnectionClose = true;

                using (var responseMessage = await client.SendAsync(requestMessage))
                {
                    var detail = await responseMessage.Content.ReadAsStringAsync();

                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return JsonSerializer.Deserialize<T>(detail, jsonSerializeOptions);
                    }
                    else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        throw new ApiResponseException(JsonSerializer.Deserialize<ProblemDetails>(detail, jsonSerializeOptions));
                    }
                }
            }
        }

        protected virtual string GetAction(ApiRequest request)
        {
            switch (request.InterfazExterna)
            {
                case CInterfazExterna.Empresas:
                    return "/Empresa/CreateOrUpdate";
                case CInterfazExterna.Producto:
                    return "/Producto/CreateOrUpdate";
                case CInterfazExterna.ProductoProveedor:
                    return "/ProductoProveedor/CreateOrDelete";
                case CInterfazExterna.CodigoDeBarras:
                    return "/CodigoBarras/CreateUpdateOrDelete";
                case CInterfazExterna.Agentes:
                    return "/Agente/CreateOrUpdate";
                case CInterfazExterna.ReferenciaDeRecepcion:
                    return "/ReferenciaRecepcion/Create";
                case CInterfazExterna.ModificarDetalleReferenciaRecepcion:
                    return "/ModificarDetalleReferencia/Update";
                case CInterfazExterna.AnulacionReferenciaRecepcion:
                    return "/AnulacionReferenciaRecepcion/Update";
                case CInterfazExterna.Pedidos:
                    return "/Pedido/Create";
                case CInterfazExterna.ModificarPedido:
                    return "/ModificarPedido/Update";
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual bool? GetErrorCarga(WISDB context, ProblemDetails problemDetails)
        {
            var title = problemDetails.Title;

            if (title.Count(c => c == '\'') == 2)
            {
                title = title.Substring(title.IndexOf('\'') + 1);
                title = title.Substring(0, title.IndexOf('\''));

                if (long.TryParse(title, out long nuInterfazEjec))
                {
                    var ejec = context.T_INTERFAZ_EJECUCION
                        .AsNoTracking()
                        .FirstOrDefault(ie => ie.NU_INTERFAZ_EJECUCION == nuInterfazEjec);

                    if (ejec != null)
                    {
                        return ejec.FL_ERROR_CARGA == "S";
                    }
                }
            }

            return null;
        }
    }
}
