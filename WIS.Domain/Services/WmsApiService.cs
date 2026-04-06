using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Http;

namespace WIS.Domain.Services
{
    public class WmsApiService : IWmsApiService
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IWebApiClient _client;
        protected readonly IOptions<WmsApiSettings> _apiSettings;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _timeout;

        public WmsApiService(IHttpContextAccessor httpContextAccessor,
            IUnitOfWorkFactory uowFactory,
            IWebApiClient client,
            IOptions<WmsApiSettings> apiSettings,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _client = client;
            _apiSettings = apiSettings;
            _timeout = int.Parse(configuration.GetSection("AppSettings:InternalTimeout")?.Value ?? "30");
            _uowFactory = uowFactory;
        }

        public virtual bool IsEnabled()
        {
            return _apiSettings.Value.IsEnabled;
        }

        public virtual string CallService(string method, string content)
        {
            string success = string.Empty;

            try
            {
                HttpResponseMessage response = HttpPost(this._apiSettings.Value.Endpoint, method, content);

                if (response.IsSuccessStatusCode)
                {
                    var msg = response.Content.ReadAsStringAsync().Result.ToString();

                    var result = JsonConvert.DeserializeObject<DTOResponse>(response.Content.ReadAsStringAsync().Result);
                    success = $"Operación realizada con éxito. Nro Interfaz: {result.NumeroInterfaz}";
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var task = response.Content.ReadAsStringAsync();
                        task.Wait();

                        var msg = task.Result.ToString();
                        msg = string.IsNullOrEmpty(msg) ? "Unauthorized" : msg;

                        logger.Error($"Error CallService WMS API {method}. Status: {response.StatusCode}. Message: {msg}. Datos: {content} ");

                        throw new Exception(msg);
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var task = response.Content.ReadAsStringAsync();
                        task.Wait();

                        var result = JsonConvert.DeserializeObject<ProblemDetails>(task.Result);
                        logger.Error($"Error CallService WMS API {method}. Status: {response.StatusCode}. Message: {result.Detail ?? result.Title}. Datos: {content} ");

                        if (!string.IsNullOrEmpty(result.Detail))
                        {
                            var errors = new List<ValidationsError>();

                            try
                            {
                                errors = JsonConvert.DeserializeObject<List<ValidationsError>>(result.Detail);
                            }
                            catch
                            {
                                //Do nothing
                            }

                            if (errors.Count > 0)
                                throw new ApiEntradaValidationException(result.Title, errors);
                        }

                        throw new Exception(result.Title);
                    }
                    else
                    {
                        var task = response.Content.ReadAsStringAsync();
                        task.Wait();

                        var msg = task.Result.ToString();

                        logger.Error($"Error CallService WMS API {method}. Status: {response.StatusCode}. Message: {msg}. Datos: {content} ");

                        ApiError apiError = null;

                        try
                        {
                            apiError = JsonConvert.DeserializeObject<ApiError>(msg);
                        }
                        catch
                        {
                            //Do nothing
                        }

                        if (apiError != null)
                            throw new ApiException(msg = apiError.Message + " " + apiError.LogId);

                        throw new Exception(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error CallService WMS API {method}. Error: {ex}. Datos: {content} ");
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    throw new Exception(ex.InnerException.Message);
                else
                    throw ex;
            }

            return success;
        }

        public virtual HttpResponseMessage HttpPost(string endpoint, string method, string content)
        {
            var address = new Uri(new Uri(endpoint), method);

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
                RequestUri = address,
            };

            request.Headers.ConnectionClose = true;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.UseDefaultCredentials = false;

            using (var client = new HttpClient(clientHandler))
            {
                var access_token = _httpContextAccessor.HttpContext.Request.Headers["access_token"][0];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(_timeout);

                var response = client.SendAsync(request).Result;

                return response;
            }

        }

        public class DTOResponse
        {
            public int CodigoEmpresa { get; set; }
            public int CodigoInterfaz { get; set; }
            public long NumeroInterfaz { get; set; }
        }
    }
}
