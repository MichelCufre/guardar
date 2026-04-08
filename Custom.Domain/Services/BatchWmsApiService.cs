using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using WIS.Domain.DataModel;
using WIS.Domain.Services;
using WIS.Domain.Services.Configuracion;
using WIS.Http;

namespace Custom.Domain.Services
{
    /// <summary>
    /// Extiende WmsApiService para funcionar en contexto batch (sin HttpContext).
    /// Sobrescribe HttpPost para inyectar el token externamente en lugar de leerlo del header HTTP.
    /// El metodo CallService con todo su manejo de errores se reutiliza intacto desde la clase base.
    /// </summary>
    public class BatchWmsApiService : WmsApiService
    {
        private string _accessToken;

        public BatchWmsApiService(
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWorkFactory uowFactory,
            IWebApiClient client,
            IOptions<WmsApiSettings> apiSettings,
            IConfiguration configuration)
            : base(httpContextAccessor, uowFactory, client, apiSettings, configuration)
        {
        }

        /// <summary>
        /// Establece el token a usar en las llamadas HTTP.
        /// Debe llamarse antes de invocar CallService.
        /// </summary>
        public void SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }

        /// <summary>
        /// Sobrescribe HttpPost para usar el token inyectado en lugar del HttpContext.
        /// El resto del comportamiento (manejo de errores, parsing) lo maneja CallService en la clase base.
        /// </summary>
        public override HttpResponseMessage HttpPost(string endpoint, string method, string content)
        {
            var address = new Uri(new Uri(endpoint), method);

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new System.Net.Http.StringContent(content, System.Text.Encoding.UTF8, "application/json"),
                RequestUri = address,
            };

            request.Headers.ConnectionClose = true;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            clientHandler.UseDefaultCredentials = false;

            using (var client = new HttpClient(clientHandler))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(_timeout);

                return client.SendAsync(request).Result;
            }
        }
    }
}
