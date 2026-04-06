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
    public class PedidosAnuladosController : ControllerBaseExtension
    {
        private readonly ILogger<PedidosAnuladosController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.PedidosAnulados;

        public PedidosAnuladosController(ILogger<PedidosAnuladosController> logger, 
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _logger = logger;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>
        ///     swagger_summary_pedidosanulados_getdata
        /// </summary>
        /// <remarks>swagger_remarks_pedidosanulados_getdata</remarks>
        /// <param name="nroEjecucion" example="4564">swagger_param_nroEjecucion_pedidosanulados_getdata</param>
        /// <param name="empresa" example="1">swagger_param_empresa_pedidosanulados_getdata</param>
        /// <returns>swagger_returns_pedidosanulados_getdata</returns>
        /// <response code="200">swagger_response_200_pedidosanulados_getdata</response>
        /// <response code="401">swagger_response_401_pedidosanulados_getdata</response>
        /// <response code="404">swagger_response_404_pedidosanulados_getdata</response>
        [HttpGet("GetData")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PedidosAnuladosResponse))]
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
                        error = new Error("WMSAPI_msg_Error_APIIncorrecta", "Pedidos anulados");
                        return Problem400BadRequest(_validationService.Translate(error));
                    }

                    var interfazData = await _ejecucionService.GetEjecucionData(nroEjecucion);

                    if (interfazData != null)
                    {
                        string data = System.Text.Encoding.UTF8.GetString(interfazData.Data);
                        var response = JsonConvert.DeserializeObject<PedidosAnuladosResponse>(data);
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
