using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WIS.Domain.Tracking.Config;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Tracking.Validation;

namespace WIS.Domain.Tracking
{
    public class APITrackingService : IAPITrackingService
    {
        protected readonly ITrackingConfigProvider _trackingConfigProvider;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public APITrackingService(ITrackingConfigProvider iTrackingConfigProvider)
        {
            this._trackingConfigProvider = iTrackingConfigProvider;
        }

        public virtual bool TrackingHabilitado()
        {
            return _trackingConfigProvider.TrackingHabilitado(); ;
        }
        public virtual Dictionary<string, string> GetConfig()
        {
            return _trackingConfigProvider.GetConfig();
        }

        #region HttpMethod

        public virtual HttpResponseMessage HttpPost(string method, string content, bool apiUser = false)
        {
            string api;
            var config = this._trackingConfigProvider.GetConfig();

            if (apiUser)
                api = config["urlApiUsers"];
            else
                api = config["url"];

            var uri = $"{api}/{config["tenant"]}/{method}";
            var address = new Uri(uri);

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

            logger.Info($"Uri : {request.RequestUri}");
            logger.Info($"Content : {content}");

            using (var client = new HttpClient(clientHandler))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenExplained(config));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.SendAsync(request).Result;

                logger.Info($"Response: {response}");

                return response;
            }

        }
        public virtual HttpResponseMessage HttpGet(string method, Dictionary<string, string> content)
        {
            var config = this._trackingConfigProvider.GetConfig();
            var uri = $"{config["url"]}/{config["tenant"]}/{method}";
            var queryString = string.Join("&", content.Select(s => s.Key + "=" + s.Value).ToList());
            var address = new Uri($"{uri}?{queryString}");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = address,
            };

            request.Headers.ConnectionClose = true;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.UseDefaultCredentials = false;

            logger.Info($"Uri : {request.RequestUri}");

            using (var client = new HttpClient(clientHandler))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenExplained(config));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.SendAsync(request).Result;
                logger.Info($"Response: {response}");

                return response;
            }
        }
        public virtual HttpResponseMessage HttpPut(string method, string content, bool apiUser = false)
        {
            string api;
            var config = this._trackingConfigProvider.GetConfig();

            if (apiUser)
                api = config["urlApiUsers"];
            else
                api = config["url"];

            var uri = $"{api}/{config["tenant"]}/{method}";
            var address = new Uri(uri);

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
                RequestUri = address,
            };

            request.Headers.ConnectionClose = true;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.UseDefaultCredentials = false;

            logger.Info($"Uri : {request.RequestUri}");
            logger.Info($"Content : {content}");

            using (var client = new HttpClient(clientHandler))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenExplained(config));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.SendAsync(request).Result;
                logger.Info($"Response: {response}");

                return response;
            }
        }

        public virtual string GetTokenExplained(Dictionary<string, string> config)
        {
            string creds = String.Format("{0}:{1}", config["clientId"], WebUtility.UrlEncode(config["clientSecret"]));
            byte[] bytes = Encoding.UTF8.GetBytes(creds);

            var data = new Dictionary<string, string>
            {
                { "grant_type", config["granType"] },
                { "scope", config["scope"] }
            };

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.UseDefaultCredentials = false;

            var client = new HttpClient(clientHandler);
            var message = new HttpRequestMessage(HttpMethod.Post, config["urlAccesToken"]);
            message.Content = new FormUrlEncodedContent(data);
            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

            message.Headers.ConnectionClose = true;

            var response = client.SendAsync(message).Result;

            string result = response.Content.ReadAsStringAsync().Result;

            var json = JObject.Parse(result);

            if (json.TryGetValue("access_token", out JToken token))
                return token.ToString();

            return "";
        }
        public virtual TrackingValidationResult GetValidationResult(HttpResponseMessage response)
        {
            TrackingValidationResult result = new TrackingValidationResult();

            result.Code = response.StatusCode.ToString();
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Conflict:

                    var jsonResult = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                    if (jsonResult.TryGetValue("detail", out JToken detalle))
                        result.AddError(detalle.ToString());
                    else
                        result.AddGenericError();
                    break;
                case HttpStatusCode.BadRequest:
                    var json = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                    if (json.TryGetValue("errors", out JToken detalles))
                    {
                        var errores = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(detalles.ToString());
                        foreach (var error in errores)
                        {
                            result.AddError($"{error.Key}: {error.Value.FirstOrDefault()}");
                        }
                    }
                    else
                        result.AddGenericError();
                    break;
                case HttpStatusCode.Unauthorized:
                    result.AddError(response.Content.ReadAsStringAsync().Result);
                    break;
                case HttpStatusCode.InternalServerError:
                default:
                    result.AddGenericError();
                    break;
            }

            logger.Error($"{result.Code}: {response}");

            if (result.Errors != null && result.Errors.Any())
                logger.Info("GetValidationResult Mensaje: " + result.Errors.FirstOrDefault().Mensaje);

            return result;
        }

        #region TestConexion
        public virtual bool GetAgenteTest(Dictionary<string, string> config)
        {
            try
            {
                var content = new Dictionary<string, string>();
                content.Add("id", "00000");
                HttpResponseMessage response = this.HttpGetTest("Agente/GetById", content, config);

                if (!response.IsSuccessStatusCode)
                {
                    var result = GetValidationResult(response);
                    var codigosAceptados = new List<string>()
                    {
                        HttpStatusCode.NotFound.ToString(),
                        HttpStatusCode.Conflict.ToString(),
                        HttpStatusCode.BadRequest.ToString(),
                    };

                    if (!codigosAceptados.Contains(result.Code))
                        return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error test de conexión :" + ex);
                return false;
            }

            return true;
        }
        public virtual HttpResponseMessage HttpGetTest(string method, Dictionary<string, string> content, Dictionary<string, string> config)
        {
            var uri = $"{config["url"]}/{config["tenant"]}/{method}";
            var queryString = string.Join("&", content.Select(s => s.Key + "=" + s.Value).ToList());
            var address = new Uri($"{uri}?{queryString}");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = address,
            };

            request.Headers.ConnectionClose = true;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.UseDefaultCredentials = false;

            logger.Info($"Uri : {request.RequestUri}");

            using (var client = new HttpClient(clientHandler))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenExplainedTest(config));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.SendAsync(request).Result;
                return response;
            }
        }
        public virtual string GetTokenExplainedTest(Dictionary<string, string> config)
        {
            string creds = String.Format("{0}:{1}", config["clientId"], WebUtility.UrlEncode(config["clientSecret"]));
            byte[] bytes = Encoding.UTF8.GetBytes(creds);

            var data = new Dictionary<string, string>
            {
                { "grant_type", config["granType"] },
                { "scope", config["scope"] }
            };

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.UseDefaultCredentials = false;

            var client = new HttpClient(clientHandler);
            var message = new HttpRequestMessage(HttpMethod.Post, config["urlAccesToken"]);
            message.Content = new FormUrlEncodedContent(data);
            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

            message.Headers.ConnectionClose = true;

            var response = client.SendAsync(message).Result;

            string result = response.Content.ReadAsStringAsync().Result;

            var json = JObject.Parse(result);

            if (json.TryGetValue("access_token", out JToken token))
                return token.ToString();

            return "";
        }

        #endregion

        #endregion

        #region Agente
        public virtual TrackingValidationResult AddAgente(AgenteRequest agente)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info("Crear Agente: " + agente.Codigo);
                HttpResponseMessage response = this.HttpPost("Agente/Create", JsonConvert.SerializeObject(agente));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
            }
            catch (Exception ex)
            {
                result.AddError($"Error AddAgente: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error AddAgente - {ex}");
            }
            return result;
        }
        #endregion

        #region Objeto
        public virtual TrackingValidationResult ModificarObjeto(ModificarObjetosRequest r, out ModificarObjetosResponse res)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            res = new ModificarObjetosResponse();

            try
            {
                HttpResponseMessage response = this.HttpPost("Objeto/Modify", JsonConvert.SerializeObject(r.Objetos));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
                else
                    res.Objetos = JsonConvert.DeserializeObject<List<ModificarObjetoResponse>>(response.Content.ReadAsStringAsync().Result);

            }
            catch (Exception ex)
            {
                result.AddError($"Error ModificarObjeto: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error ModificarObjeto - {ex}");
            }
            return result;
        }
        #endregion

        #region PuntoEntrega
        public virtual TrackingValidationResult AddPuntoEntrega(PuntoDeEntregaRequest p, out string cdPuntoEntrega, out string cdZona)
        {
            cdPuntoEntrega = string.Empty;
            cdZona = string.Empty;

            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info($"Crear punto de entrega. Dirección: {p.Direccion.Direccion}");
                HttpResponseMessage response = this.HttpPost("PuntoEntrega/Create", JsonConvert.SerializeObject(p));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
                else
                {
                    var res = JsonConvert.DeserializeObject<PuntoDeEntregaResponse>(response.Content.ReadAsStringAsync().Result);
                    cdPuntoEntrega = res.Codigo;
                    cdZona = res.Direccion?.Zona;
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Error AddPuntoEntrega: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error AddPuntoEntrega - {ex}");
            }
            return result;
        }
        #endregion

        #region Tarea
        public virtual TrackingValidationResult AddTarea(TareaRequest tarea)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info($"Crear tarea. CodigoExterno: {tarea.CodigoExterno} - Punto de Entrega: {tarea.CodigoPuntoEntregaDestino} - Pedido: {tarea.Pedidos.FirstOrDefault().Numero}");
                HttpResponseMessage response = this.HttpPost("Tarea/Create", JsonConvert.SerializeObject(tarea));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
            }
            catch (Exception ex)
            {
                result.AddError($"Error AddTarea: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error AddTarea - {ex}");
            }
            return result;
        }
        public virtual TrackingValidationResult EnviarTareaConAcciones(List<PlanificacionRequest> request)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                HttpResponseMessage response = this.HttpPost("Tarea/CrearTareaConAcciones", JsonConvert.SerializeObject(request));
                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
            }
            catch (Exception ex)
            {
                result.AddError($"Error EnviarTareaConAcciones: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error EnviarTareaConAcciones - {ex}");
            }
            return result;
        }
        #endregion

        #region TareaReferencia
        public virtual TrackingValidationResult CerrarPedido(AnularPedidoRequest tareaReferencia)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info($"Cerrar pedido. Nro: {tareaReferencia.Numero} - Empresa: {tareaReferencia.CodigoEmpresa} - cdAgente: {tareaReferencia.CodigoAgente}");
                HttpResponseMessage response = this.HttpPost("TareaReferencia/CerrarPedido", JsonConvert.SerializeObject(tareaReferencia));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);

            }
            catch (Exception ex)
            {
                result.AddError($"Error CerrarPedido: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error CerrarPedido - {ex}");
            }
            return result;
        }
        #endregion

        #region TipoVehiculo
        public virtual TrackingValidationResult AddTipoVehiculo(TipoVehiculoRequest tipo)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info("Creación tipo de vehículo: " + tipo.CodigoExterno);
                var response = this.HttpPost("TipoVehiculo/Create", JsonConvert.SerializeObject(tipo));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
            }
            catch (Exception ex)
            {
                result.AddError($"Error AddTipoVehiculo: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error AddTipoVehiculo - {ex}");
            }

            return result;
        }
        public virtual TrackingValidationResult UpdateTipoVehiculo(TipoVehiculoRequest tipo)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info("Actualizar tipo de vehículo: " + tipo.CodigoExterno);
                HttpResponseMessage response = this.HttpPut("TipoVehiculo/Update", JsonConvert.SerializeObject(tipo));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
            }
            catch (Exception ex)
            {
                result.AddError($"Error UpdateTipoVehiculo: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error UpdateTipoVehiculo - {ex}");
            }
            return result;
        }
        #endregion

        #region Users
        public virtual TrackingValidationResult AddUser(UserRequest usuario)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info("Agregar usuario a tracking. UserId: " + usuario.UserId);

                usuario.Tenant = this._trackingConfigProvider.GetConfig()["tenant"];
                HttpResponseMessage response = this.HttpPost("User", JsonConvert.SerializeObject(usuario), true);

                if (!response.IsSuccessStatusCode)
                {
                    var mensaje = JsonConvert.DeserializeObject<string>(response.Content.ReadAsStringAsync().Result);
                    if (string.IsNullOrEmpty(mensaje))
                        mensaje = $"Error al crear el usuario: {usuario.UserId}";

                    result.Code = response.StatusCode.ToString();
                    result.AddError(mensaje);
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Error AddUser: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error AddUser - {ex}");

            }
            return result;
        }
        public virtual TrackingValidationResult UpdateUser(UserRequest usuario)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info("Actualizar usuario de tracking. UserId: " + usuario.UserId);
                usuario.Tenant = this._trackingConfigProvider.GetConfig()["tenant"];
                HttpResponseMessage response = this.HttpPut("User", JsonConvert.SerializeObject(usuario), true);

                if (!response.IsSuccessStatusCode)
                {
                    var mensaje = JsonConvert.DeserializeObject<string>(response.Content.ReadAsStringAsync().Result);
                    if (string.IsNullOrEmpty(mensaje))
                        mensaje = $"Error al actualizar el usuario: {usuario.UserId}";

                    result.Code = response.StatusCode.ToString();
                    result.AddError(mensaje);

                }
            }
            catch (Exception ex)
            {
                result.AddError($"Error UpdateUser: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error UpdateUser - {ex}");
            }
            return result;
        }
        #endregion

        #region Vehiculo
        public virtual TrackingValidationResult AddVehiculo(VehiculoRequest vehiculo)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info("Crear vehículo: " + vehiculo.CodigoExterno);
                HttpResponseMessage response = this.HttpPost("Vehiculo/Create", JsonConvert.SerializeObject(vehiculo));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
            }
            catch (Exception ex)
            {
                result.AddError($"Error AddVehiculo: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error AddVehiculo - {ex}");
            }
            return result;
        }
        public virtual TrackingValidationResult UpdateVehiculo(VehiculoRequest vehiculo)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info("Actualizar vehículo: " + vehiculo.CodigoExterno);
                HttpResponseMessage response = this.HttpPut("Vehiculo/Update", JsonConvert.SerializeObject(vehiculo));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
            }
            catch (Exception ex)
            {
                result.AddError($"Error UpdateVehiculo: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error UpdateVehiculo - {ex}");
            }
            return result;
        }
        #endregion

        #region Viaje
        public virtual TrackingValidationResult CrearViaje(ViajeTeoricoRequest viaje)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info("Crear viaje. Camión: " + viaje.Numero);
                HttpResponseMessage response = this.HttpPost("Viaje/CreateOrUpdate", JsonConvert.SerializeObject(viaje));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
            }
            catch (Exception ex)
            {
                result.AddError($"Error CrearViaje: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error CrearViaje - {ex}");
            }
            return result;
        }
        public virtual TrackingValidationResult ConfirmarViaje(ViajeRealRequest viaje)
        {
            TrackingValidationResult result = new TrackingValidationResult() { Code = HttpStatusCode.OK.ToString() };
            try
            {
                logger.Info("Confirmar viaje. Camión: " + viaje.Numero);
                HttpResponseMessage response = this.HttpPost("Viaje/Confirmar", JsonConvert.SerializeObject(viaje));

                if (!response.IsSuccessStatusCode)
                    result = GetValidationResult(response);
            }
            catch (Exception ex)
            {
                result.AddError($"Error ConfirmarViaje: {ex.Message}");
                result.Code = HttpStatusCode.InternalServerError.ToString();
                logger.Error($"APITrackingService - Error ConfirmarViaje - {ex}");
            }
            return result;
        }
        #endregion

    }
}
