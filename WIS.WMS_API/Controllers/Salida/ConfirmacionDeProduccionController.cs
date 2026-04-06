using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
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
    public class ConfirmacionDeProduccionController : ControllerBaseExtension
    {
        private readonly ILogger<ConfirmacionDeProduccionController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.ConfirmacionProduccion;

        public ConfirmacionDeProduccionController(ILogger<ConfirmacionDeProduccionController> logger,
        IEjecucionService ejecucionService,
        IValidationService validationService)
        {
            _logger = logger;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>
        ///     swagger_summary_confirmaciondeproduccion_getdata
        /// </summary>
        /// <remarks>swagger_remarks_confirmaciondeproduccion_getdata</remarks>
        /// <param name="nroEjecucion" example="4564">swagger_param_nroEjecucion_confirmaciondeproduccion_getdata</param>
        /// <param name="empresa" example="1">swagger_param_empresa_confirmaciondeproduccion_getdata</param>
        /// <returns>swagger_returns_confirmaciondeproduccion_getdata</returns>
        /// <response code="200">swagger_response_200_confirmaciondeproduccion_getdata</response>
        /// <response code="401">swagger_response_401_confirmaciondeproduccion_getdata</response>
        /// <response code="404">swagger_response_404_confirmaciondeproduccion_getdata</response>
        [HttpGet("GetData")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConfirmacionProduccionResponse))]
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
                        error = new Error("WMSAPI_msg_Error_APIIncorrecta", "Confirmación de Producción");
                        return Problem400BadRequest(_validationService.Translate(error));
                    }

                    var interfazData = await _ejecucionService.GetEjecucionData(nroEjecucion);
                    if (interfazData != null)
                    {
                        string data = System.Text.Encoding.UTF8.GetString(interfazData.Data);
                        var response = JsonConvert.DeserializeObject<ConfirmacionProduccionResponse>(data);
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
