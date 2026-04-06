using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WIS.WebhookClient.Models;

namespace WIS.WebhookClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class WebhookClientController : ControllerBase
    {
        private IConfiguration _configuration;
        private ILogger _logger;

        public WebhookClientController(IConfiguration configuration, ILogger<WebhookClientController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        ///     swagger_summary_webhookclient_callback
        /// </summary>
        /// <remarks>swagger_remarks_webhookclient_callback</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">swagger_response_200_webhookclient_callback</response>
        /// <response code="400">swagger_response_400_webhookclient_callback</response>
        /// <response code="401">swagger_response_401_webhookclient_callback</response>
        [HttpPost("Callback")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Callback([FromBody] EventRequest request)
        {
            if (await IsValidSignature())
            {
                _logger.LogTrace($"Nro. Interfaz  ejecución: {request.NumeroInterfazEjecucion}");

                switch (request.Id)
                {
                    case "ConfirmacionRecepcion": Process(request.ConfirmacionRecepcion); break;
                    case "ConfirmacionPedido": Process(request.ConfirmacionPedido); break;
                    case "PedidosAnulados": Process(request.PedidosAnulados); break;
                    case "Ajustes": Process(request.Ajustes); break;
                    case "ConsultaStock": Process(request.ConsultaStock); break;
                    case "ConfirmacionMercaderiaPreparada": Process(request.ConfirmacionMercaderiaPreparada); break;
                    case "Almacenamiento": Process(request.Almacenamiento); break;
                    case "Test": Process(request.Test); break;
					case "ConfirmacionProduccion": Process(request.ConfirmacionProduccion); break;
					default: return BadRequest("Evento desconocido");
                }

                return Ok("Operación realizada con éxito.");
            }
            else
            {
                return Unauthorized("Operación no autorizada");
            }
        }

		private void Process(ConfirmacionMercaderiaPreparadaRequest request)
        {
            //TODO:
            _logger.LogTrace(JsonSerializer.Serialize(request));
        }

        private void Process(ConsultaStockRequest request)
        {
            //TODO:
            _logger.LogTrace(JsonSerializer.Serialize(request));
        }

        private void Process(AjustesRequest request)
        {
            //TODO:
            _logger.LogTrace(JsonSerializer.Serialize(request));
        }

        private void Process(PedidosAnuladosRequest request)
        {
            //TODO:
            _logger.LogTrace(JsonSerializer.Serialize(request));
        }

        private void Process(ConfirmacionPedidoRequest request)
        {
            //TODO:
            _logger.LogTrace(JsonSerializer.Serialize(request));
        }

        private void Process(AgendaRequest request)
        {
            //TODO:
            _logger.LogTrace(JsonSerializer.Serialize(request));
        }

        private void Process(AlmacenamientoRequest request)
        {
            //TODO:
            _logger.LogTrace(JsonSerializer.Serialize(request));
        }

        private void Process(TestRequest request)
        {
            //TODO:
            _logger.LogTrace(JsonSerializer.Serialize(request));
        }

		private void Process(ConfirmacionProduccionRequest request)
		{
			// TODO:
            _logger.LogTrace(JsonSerializer.Serialize(request));
		}

		private async Task<bool> IsValidSignature()
        {
            var payload = string.Empty;

            Request.Body.Position = 0;

            using (var sr = new StreamReader(Request.Body, Encoding.UTF8))
            {
                payload = await sr.ReadToEndAsync();
            }
            _logger.LogTrace(payload);

            if (Request.Headers.TryGetValue("X-Hub-Signature", out StringValues xHubSignature))
            {
                if (xHubSignature.Count > 0)
                {
                    var secret = _configuration.GetValue<string>("AuthSettings:WebhookSecret");
                    var hash = ComputeHash(secret, payload);
                    var signature = Convert.FromBase64String(xHubSignature[0]);

                    return CryptographicOperations.FixedTimeEquals(hash, signature);
                }
            }

            return false;
        }

        public static byte[] ComputeHash(string secret, string payload)
        {
            var bytes = Encoding.UTF8.GetBytes(secret);
            using (var hmac = new HMACSHA512(bytes))
            {
                bytes = Encoding.UTF8.GetBytes(payload);
                return hmac.ComputeHash(bytes);
            }
        }
    }

}
