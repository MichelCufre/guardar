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
    public class AjustesDeStockController : ControllerBaseExtension
    {
        private readonly ILogger<AjustesDeStockController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.AjustesDeStock;

        public AjustesDeStockController(ILogger<AjustesDeStockController> logger, 
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _logger = logger;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>
        ///     swagger_summary_ajustesdestock_getdata
        /// </summary>
        /// <remarks>swagger_remarks_ajustesdestock_getdata</remarks>
        /// <param name="nroEjecucion" example="4564">swagger_param_nroEjecucion_ajustesdestock_getdata</param>
        /// <param name="empresa" example="1">swagger_param_empresa_ajustesdestock_getdata</param>
        /// <returns>swagger_returns_ajustesdestock_getdata</returns>
        /// <response code="200">swagger_response_200_ajustesdestock_getdata</response>
        /// <response code="401">swagger_response_401_ajustesdestock_getdata</response>
        /// <response code="404">swagger_response_404_ajustesdestock_getdata</response>
        [HttpGet("GetData")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AjustesResponse))]
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
                        error = new Error("WMSAPI_msg_Error_APIIncorrecta", "Ajustes de Stock");
                        return Problem400BadRequest(_validationService.Translate(error));
					}

                    var interfazData = await _ejecucionService.GetEjecucionData(nroEjecucion);
                    if (interfazData != null)
                    {
                        string data = System.Text.Encoding.UTF8.GetString(interfazData.Data);
                        var response = JsonConvert.DeserializeObject<AjustesResponse>(data);
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
