using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WIS.Configuration;
using WIS.Domain.General;
using WIS.Domain.Security;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IOptions<AuthSettings> _configuration;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public AuthorizationService(IHttpContextAccessor httpContextAccessor, IOptions<AuthSettings> configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public virtual void ChangePassword(ChangePassword model)
        {
            var method = "Password/Change";
            var content = JsonConvert.SerializeObject(model);
            this.CallService(method, content);
        }
        public virtual void ValidateCurrentPassword(ValidateCurrentPassword model)
        {
            var method = "Password/ValidateCurrentPassword";
            var content = JsonConvert.SerializeObject(model);
            this.CallService(method, content);
        }

        public virtual string CallService(string method, string content)
        {
            string success = string.Empty;

            try
            {
                HttpResponseMessage response = HttpPost(this._configuration.Value.AuthorizationApi, method, content);

                if (response.IsSuccessStatusCode)
                {
                    var msg = response.Content.ReadAsStringAsync().Result.ToString();
                    success = $"Operación realizada con éxito.";
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var task = response.Content.ReadAsStringAsync();
                        task.Wait();

                        var msg = task.Result.ToString();
                        msg = string.IsNullOrEmpty(msg) ? "Unauthorized" : msg;

                        logger.Error($"Error CallService AuthorizationAPI {method}. Status: {response.StatusCode}. Message: {msg}. Datos: {content} ");

                        throw new Exception(msg);
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var task = response.Content.ReadAsStringAsync();
                        task.Wait();

                        var result = JsonConvert.DeserializeObject<ProblemDetails>(task.Result);
                        logger.Error($"Error CallService AuthorizationAPI {method}. Status: {response.StatusCode}. Message: {result.Detail ?? result.Title}. Datos: {content} ");

                        string msg = string.Empty;
                        if (!string.IsNullOrEmpty(result.Detail))
                        {
                            var errors = JsonConvert.DeserializeObject<List<ValidationsError>>(result.Detail);

                            if (errors.Count > 0)
                            {
                                foreach (var error in errors)
                                {
                                    msg += string.Join(" - ", error.Messages.ToArray());
                                }
                            }
                        }
                        msg = !string.IsNullOrEmpty(msg) ? msg : result.Title;
                        throw new Exception(msg);
                    }
                    else
                    {
                        var task = response.Content.ReadAsStringAsync();
                        task.Wait();

                        var msg = !string.IsNullOrEmpty(task.Result.ToString()) ? task.Result.ToString() : "Error no controlado.";
                        logger.Error($"Error CallService AuthorizationAPI {method}. Status: {response.StatusCode}. Message: {msg}. Datos: {content} ");
                        throw new Exception(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error CallService AuthorizationAPI {method}. Error: {ex}. Datos: {content} ");
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
                var response = client.SendAsync(request).Result;

                return response;
            }

        }

    }
}
