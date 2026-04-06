using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AlmacenamientoController : ControllerBaseExtension
    {
        private readonly ILogger<PedidosAnuladosController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private const int _interfazExterna = CInterfazExterna.Almacenamiento;

        public AlmacenamientoController(ILogger<PedidosAnuladosController> logger, IEjecucionService ejecucionService)
        {
            _logger = logger;
            _ejecucionService = ejecucionService;
        }

		/// <summary>
		///     swagger_summary_almacenamiento_getdata
		/// </summary>
		/// <remarks>swagger_remarks_almacenamiento_getdata</remarks>
		/// <param name="nroEjecucion" example="4564">swagger_param_nroEjecucion_almacenamiento_getdata</param>
		/// <param name="empresa" example="1">swagger_param_empresa_almacenamiento_getdata</param>
		/// <returns>swagger_returns_almacenamiento_getdata</returns>
		/// <response code="200">swagger_response_200_almacenamiento_getdata</response>
		/// <response code="401">swagger_response_401_almacenamiento_getdata</response>
		/// <response code="404">swagger_response_404_almacenamiento_getdata</response>
		[HttpGet("GetData")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlmacenamientoResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetData([RequiredValidation] long nroEjecucion, [RequiredValidation] int empresa)
        {
            try
            {
                var interfaz = await _ejecucionService.GetEjecucion(nroEjecucion);
                if (interfaz != null)
                {
                    if (interfaz.CdInterfazExterna != _interfazExterna)
                        return Problem400BadRequest(string.Format(ValidationMessage.WMSAPI_msg_Error_APIIncorrecta, "Almacenamiento"));

                    var interfazData = await _ejecucionService.GetEjecucionData(nroEjecucion);
                    if (interfazData != null)
                    {
                        string data = System.Text.Encoding.UTF8.GetString(interfazData.Data);
                        var response = JsonConvert.DeserializeObject<AlmacenamientoResponse>(data);
                        return Ok(response);
                    }
                    else
                        return Problem404NotFound(string.Format(ValidationMessage.WMSAPI_msg_Error_InterfazDataNoEncontrada, nroEjecucion));
                }
                return Problem404NotFound(string.Format(ValidationMessage.WMSAPI_msg_Error_InterfazNoExiste, nroEjecucion));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }
    }
}
