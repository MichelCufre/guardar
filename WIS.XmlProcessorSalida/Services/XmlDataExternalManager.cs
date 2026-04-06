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
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.Persistence.Database;
using WIS.XmlProcessorSalida.Helpers;
using WIS.XmlProcessorSalida.Models;
using WIS.XmlProcessorSalida.Services.Interfaces;

namespace WIS.XmlProcessorSalida.Services
{
    public class XmlDataExternalManager : IXmlDataExternalManager
    {
        protected ApiMapper _mapper;
        protected XmlProcessorCommon _xmlCommonProcess;
        protected readonly IConfiguration _configuration;
        protected readonly ILogger<XmlDataExternalManager> _logger;
        protected readonly IOptions<DatabaseSettings> _dbSettings;
        protected readonly IDatabaseConfigurationService _dbConfigService;
        protected readonly IDapper _dapper;

        public XmlDataExternalManager(IConfiguration configuration,
            ILogger<XmlDataExternalManager> logger,
            IOptions<DatabaseSettings> dbSettings,
            IDatabaseConfigurationService dbConfigService,
            IDapper dapper)
        {
            _configuration = configuration;
            _logger = logger;
            _dbSettings = dbSettings;
            _dbConfigService = dbConfigService;
            _dapper = dapper;

            Initialize();
        }

        public async Task Start()
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
                    var empresas = GetEmpresas(context, userId);

                    Initialize(context);

                    foreach (var empresa in empresas)
                    {
                        _logger.LogDebug($"Obteniendo salidas de la empresa {empresa}");

                        try
                        {
                            var salidas = await GetSalidasPendientesAsync(wmsApiEndpoint, token, empresa);

                            foreach (var salida in salidas)
                            {
                                var lecturaCorrecta = true;
                                var error = string.Empty;

                                _logger.LogDebug($"Obteniendo detalle de la salida {salida.NumeroInterfazEjecucion}");

                                try
                                {
                                    var data = await GetDataAsync(wmsApiEndpoint, token, empresa, salida);

                                    _logger.LogDebug($"Mapeando detalle de la salida {salida.NumeroInterfazEjecucion}");

                                    var xml = _mapper.Map(salida.CodigoInterfazExterna, data);

                                    _logger.LogDebug($"Generando interfaz XML para la salida {salida.NumeroInterfazEjecucion}");

                                    var interfazGenerada = _xmlCommonProcess.CrearEjecucionSalida(context, _dapper, userId, empresa, salida, xml);

                                    if (!interfazGenerada)
                                    {
                                        _logger.LogDebug($"Interfaz XML no generada para la salida {salida.NumeroInterfazEjecucion} porque ya existe otra previamente generada");
                                    }
                                }
                                catch (NotImplementedException)
                                {
                                    lecturaCorrecta = false;
                                    error = $"Interfaz XML no implementada";
                                }
                                catch (ApiResponseException ex)
                                {
                                    var problemDetails = ex.ProblemDetails;

                                    lecturaCorrecta = false;
                                    error = problemDetails.Detail;

                                    _logger.LogError($"{problemDetails.Title}. Detalles: {problemDetails.Detail}");
                                }
                                catch (Exception ex)
                                {
                                    lecturaCorrecta = false;
                                    error = ex.Message;

                                    _logger.LogError(ex, "Error");
                                }

                                _logger.LogDebug($"Confirmando lectura de la salida {salida.NumeroInterfazEjecucion}");

                                await ConfirmarLecturaAsync(wmsApiEndpoint, token, empresa, salida.NumeroInterfazEjecucion, salida.CodigoInterfazExterna, lecturaCorrecta, error);

                                _logger.LogDebug($"Lectura confirmada para la salida {salida.NumeroInterfazEjecucion}");
                            }
                        }
                        catch (ApiResponseException ex)
                        {
                            var problemDetails = ex.ProblemDetails;
                            _logger.LogError($"{problemDetails.Title}. Detalles: {problemDetails.Detail}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error");
                        }
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
            _xmlCommonProcess = new XmlProcessorCommon();
        }

        protected virtual void Initialize(WISDB context)
        {
            string paramLimitPack = (string)Helpers.ParamManager.GetParamValue(context, Helpers.ParamManager.LIMIT_PACK_WS);

            if (paramLimitPack == null)
            {
                throw new Exception("No existe parámetro LIMIT_PACK_WS en la base de datos.");
            }

            if (!paramLimitPack.Equals("N"))
                int.TryParse(paramLimitPack.ToString(), out XmlProcessorCommon.MaxSizePerPackage);
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

        protected virtual WISDB GetNewDbContext()
        {
            return new WISDB(_dbConfigService, _dbSettings.Value.ConnectionString, _dbSettings.Value.Schema);
        }

        protected virtual List<int> GetEmpresas(WISDB context, int userId)
        {
            return context.T_EMPRESA_FUNCIONARIO
                .AsNoTracking()
                .Where(ef => ef.USERID == userId)
                .OrderBy(ef => ef.CD_EMPRESA)
                .Select(ef => ef.CD_EMPRESA)
                .ToList();
        }

        protected virtual int GetUserId(WISDB context, string clientId)
        {
            return context.USERS.FirstOrDefault(u => u.LOGINNAME.ToLower().Trim() == clientId.ToLower().Trim()).USERID;
        }

        protected virtual async Task<List<EjecucionPendienteResponse>> GetSalidasPendientesAsync(string baseUri, string token, int empresa)
        {
            var uriString = baseUri + "/Salida/GetEjecucionesPendientes";
            var response = await InvokeAsync<EjecucionesPendientesResponse>(uriString, HttpMethod.Get, token, new
            {
                empresa = empresa
            });

            return response.Ejecuciones;
        }

        protected virtual async Task<object> GetDataAsync(string baseUri, string token, int empresa, EjecucionPendienteResponse salida)
        {
            var interfazExterna = salida.CodigoInterfazExterna;
            var nuEjecucion = salida.NumeroInterfazEjecucion;

            switch (interfazExterna)
            {
                case CInterfazExterna.ConfirmacionDeRecepcion:
                    return await GetDataAsync<ConfirmacionRecepcionResponse>(baseUri, "ConfirmacionDeRecepcion", token, empresa, nuEjecucion);
                case CInterfazExterna.ConfirmacionDePedido:
                    return await GetDataAsync<ConfirmacionPedidoResponse>(baseUri, "ConfirmacionDePedido", token, empresa, nuEjecucion);
                case CInterfazExterna.PedidosAnulados:
                    return await GetDataAsync<PedidosAnuladosResponse>(baseUri, "PedidosAnulados", token, empresa, nuEjecucion);
                case CInterfazExterna.Facturacion:
                    return await GetDataAsync<FacturacionResponse>(baseUri, "Facturacion", token, empresa, nuEjecucion);
                case CInterfazExterna.AjustesDeStock:
                    return await GetDataAsync<AjustesResponse>(baseUri, "AjustesDeStock", token, empresa, nuEjecucion);
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual async Task<T> GetDataAsync<T>(string baseUri, string controllerName, string token, int empresa, long nuEjecucion)
        {
            var uriString = $"{baseUri}/{controllerName}/GetData";
            return await InvokeAsync<T>(uriString, HttpMethod.Get, token, new
            {
                nroEjecucion = nuEjecucion,
                empresa = empresa
            });
        }

        protected virtual async Task ConfirmarLecturaAsync(string baseUri, string token, int empresa, long nuEjecucion, int interfazExterna, bool lecturaCorrecta, string error)
        {
            var uriString = $"{baseUri}/Salida/ConfirmarLectura";
            var errores = new List<string>();

            if (!lecturaCorrecta && !string.IsNullOrEmpty(error))
                errores.Add(error);

            await InvokeAsync<object>(uriString, HttpMethod.Post, token, new ConfirmarLecturaRequest
            {
                Empresa = empresa,
                NumeroInterfazEjecucion = nuEjecucion,
                CodigoInterfazExterna = interfazExterna,
                Resultado = lecturaCorrecta,
                Errores = errores,
            });
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
    }
}
