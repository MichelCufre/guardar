using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using WIS.Automation.Galys;
using WIS.AutomationInterpreter.Extensions;
using WIS.AutomationInterpreter.Models.Mappers.Interfaces;
using WIS.AutomationInterpreter.Services;

namespace WIS.AutomationInterpreter.Controllers.Salida
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class GalysNotificationController : AutomatismoBaseController
    {
        protected AutomatismoClientService _automatismoClientService;
        protected IGalysMapper _mapper;

        public GalysNotificationController(
            ILogger<NotificacionAutomatismoController> logger,
            AutomatismoClientService automatismoClientService,
            IGalysMapper mapper) : base(logger)
        {
            this._automatismoClientService = automatismoClientService;
            this._mapper = mapper;
        }

        /// <summary>
        ///     swagger_summary_galysnotification_notificarajustes
        /// </summary>
        /// <remarks>swagger_remarks_galysnotification_notificarajustes</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_galysnotification_notificarajustes</returns>
        /// <response code="200">swagger_response_200_galysnotification_notificarajustes</response>
        /// <response code="404">swagger_response_404_galysnotification_notificarajustes</response>
        [HttpPut("NotificarAjustes")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GalysResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GalysResponse))]
		public async Task<IActionResult> NotificarAjustes([FromBody] NotificacionAjusteStockGalysRequest request)
		{
			var response = new GalysResponse();

            try
            {
                await LogRequest();
                return Ok(_automatismoClientService.NotificarAjuste(_mapper.Map(request)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"NotificarAjustes > Error");
                response.SetError(ex.Message);
                return BadRequest(response);
            }

        }

        /// <summary>
        ///     swagger_summary_galysnotification_confirmarentradas
        /// </summary>
        /// <remarks>swagger_remarks_galysnotification_confirmarentradas</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_galysnotification_confirmarentradas</returns>
        /// <response code="200">swagger_response_200_galysnotification_confirmarentradas</response>
        /// <response code="404">swagger_response_404_galysnotification_confirmarentradas</response>
        [HttpPost("ConfirmarOrdenEntrada")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GalysResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GalysResponse))]
		public async Task<IActionResult> ConfirmarOrdenEntrada([FromBody] ConfirmacionEntradaStockGalysRequest request)
		{
			var response = new GalysResponse();

            try
            {
                await LogRequest();
                return Ok(_automatismoClientService.ConfirmarEntrada(_mapper.Map(request)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ConfirmarEntradas > Error");
                response.SetError(ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        ///     swagger_summary_galysnotification_confirmarsalida
        /// </summary>
        /// <remarks>swagger_remarks_galysnotification_confirmarsalida</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_galysnotification_confirmarsalida</returns>
        /// <response code="200">swagger_response_200_galysnotification_confirmarsalida</response>
        /// <response code="404">swagger_response_404_galysnotification_confirmarsalida</response>
        [HttpPost("ConfirmarOrdenSalida")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GalysResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GalysResponse))]
		public async Task<IActionResult> ConfirmarOrdenSalida([FromBody] ConfirmacionSalidaStockGalysRequest request)
		{
			var response = new GalysResponse();

			try
			{
				await LogRequest();
				return Ok(_automatismoClientService.ConfirmarSalida(_mapper.Map(request)));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"ConfirmarSalida > Error");
				response.SetError(ex.Message);
				return BadRequest(response);
			}
		}

        /// <summary>
        ///     swagger_summary_galysnotification_confirmarmovimiento
        /// </summary>
        /// <remarks>swagger_remarks_galysnotification_confirmarmovimiento</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_galysnotification_confirmarmovimiento</returns>
        /// <response code="200">swagger_response_200_galysnotification_confirmarmovimiento</response>
        /// <response code="404">swagger_response_404_galysnotification_confirmarmovimiento</response>
        [HttpPost("ConfirmarMovimiento")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GalysResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GalysResponse))]
        public async Task<IActionResult> ConfirmarMovimiento([FromBody] ConfirmacionMovimientoStockGalysRequest request)
        {
            var response = new GalysResponse();

            try
            {
                await LogRequest();
                return Ok(_automatismoClientService.ConfirmarMovimiento(_mapper.Map(request)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ConfirmarMovimiento > Error");
                response.SetError(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
