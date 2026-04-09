using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using Custom.Domain.Services.Configuration;

namespace Custom.Domain.Services
{
    /// <summary>
    /// Cliente HTTP para el web service SOAP WsGenQuery del cliente.
    /// Llama a GetData con una keyCode y devuelve el XML de respuesta ya parseado.
    /// </summary>
    public class WsGenQueryClient
    {
        private readonly IOptions<ErpClientSettings> _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WsGenQueryClient> _logger;

        public WsGenQueryClient(
            IOptions<ErpClientSettings> settings,
            IHttpClientFactory httpClientFactory,
            ILogger<WsGenQueryClient> logger)
        {
            _settings          = settings;
            _httpClientFactory = httpClientFactory;
            _logger            = logger;
        }

        /// <summary>
        /// Llama a WsGenQuery.GetData y devuelve el XML resultado.
        /// </summary>
        /// <param name="keyCode">Clave del query, ej: "ITEMS", "AGENTES", "PEDIDOS".</param>
        /// <param name="parameters">XML de parametros, ej: "&lt;params&gt;&lt;/params&gt;".</param>
        public XDocument GetData(string keyCode, string parameters = "<params></params>")
        {
            _logger.LogDebug("WsGenQuery GetData keyCode={KeyCode}", keyCode);

            var soapBody = BuildSoapEnvelope(keyCode, parameters);
            var url      = _settings.Value.WsGenQueryUrl;

            using var client  = _httpClientFactory.CreateClient();
            using var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "\"GetData\"");

            var response = client.PostAsync(url, content).Result;
            var xml      = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"WsGenQuery error HTTP {(int)response.StatusCode}: {xml}");

            var doc = XDocument.Parse(xml);

            // Verificar si el servicio devolvio un nodo <error>
            var errorNode = doc.Descendants("error").FirstOrDefault();
            if (errorNode != null)
            {
                var code    = errorNode.Element("code")?.Value;
                var message = errorNode.Element("message")?.Value;
                throw new InvalidOperationException($"WsGenQuery error {code}: {message}");
            }

            _logger.LogDebug("WsGenQuery GetData keyCode={KeyCode} OK", keyCode);
            return doc;
        }

        private static string BuildSoapEnvelope(string keyCode, string parameters)
        {
            // Envelope SOAP 1.1 estandar para WsGenQuery
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <GetData xmlns=""http://tempuri.org/"">
      <keyCode>{keyCode}</keyCode>
      <parameters>{SecurityElement(parameters)}</parameters>
    </GetData>
  </soap:Body>
</soap:Envelope>";
        }

        private static string SecurityElement(string value)
            => value?.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;") ?? string.Empty;
    }
}
