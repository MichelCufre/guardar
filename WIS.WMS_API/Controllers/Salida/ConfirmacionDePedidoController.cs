using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.WMS_API.Extensions;

namespace WIS.WMS_API.Controllers.Salida
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ConfirmacionDePedidoController : ControllerBaseExtension
    {
        private readonly ILogger<ConfirmacionDePedidoController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.ConfirmacionDePedido;

        public ConfirmacionDePedidoController(ILogger<ConfirmacionDePedidoController> logger,
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _logger = logger;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>
        ///     swagger_summary_confirmaciondepedido_getdata
        /// </summary>
        /// <remarks>swagger_remarks_confirmaciondepedido_getdata</remarks>
        /// <param name="nroEjecucion" example="4564">swagger_param_nroEjecucion_confirmaciondepedido_getdata</param>
        /// <param name="empresa" example="1">swagger_param_empresa_confirmaciondepedido_getdata</param>
        /// <returns>swagger_returns_confirmaciondepedido_getdata</returns>
        /// <response code="200">swagger_response_200_confirmaciondepedido_getdata</response>
        /// <response code="401">swagger_response_401_confirmaciondepedido_getdata</response>
        /// <response code="404">swagger_response_404_confirmaciondepedido_getdata</response>
        [HttpGet("GetData")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConfirmacionPedidoResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetData([RequiredValidation] long nroEjecucion, [RequiredValidation] int empresa)
        {
            try
            {
                var error = new Error("");
                var interfaz = await _ejecucionService.GetEjecucion(nroEjecucion);

                if (interfaz != null)
                {
                    if (interfaz.CdInterfazExterna != _interfazExterna)
					{
                        error = new Error("WMSAPI_msg_Error_APIIncorrecta", "Confirmación de pedidos");
                        return Problem400BadRequest(_validationService.Translate(error));
					}

                    var interfazData = await _ejecucionService.GetEjecucionData(nroEjecucion);

                    if (interfazData != null)
                    {
                        string data = System.Text.Encoding.UTF8.GetString(interfazData.Data);
                        var response = JsonConvert.DeserializeObject<ConfirmacionPedidoResponse>(data);

                        return Ok(response);
                    }
					else
					{
                        error = new Error("WMSAPI_msg_Error_InterfazDataNoEncontrada", nroEjecucion);
                        return Problem404NotFound(_validationService.Translate(error));
					}
                }

                error = new Error("WMSAPI_msg_Error_InterfazNoExiste", nroEjecucion);

                return Problem404NotFound(_validationService.Translate(error));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }
    }
}
