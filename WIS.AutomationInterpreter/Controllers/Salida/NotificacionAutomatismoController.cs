using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using WIS.Automation;
using WIS.AutomationInterpreter.Extensions;
using WIS.AutomationInterpreter.Models;
using WIS.AutomationInterpreter.Models.Mappers.Interfaces;
using WIS.AutomationInterpreter.Services;
using WIS.Domain.DataModel.Mappers.Automatismo;

namespace WIS.AutomationInterpreter.Controllers.Salida
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class NotificacionAutomatismoController : AutomatismoBaseController
    {
        private AutomatismoClientService _automatismoClientService;
		private IAutomatismoMapper _mapper;

		public NotificacionAutomatismoController(
            ILogger<NotificacionAutomatismoController> logger,
            AutomatismoClientService automatismoClientService,
			IAutomatismoMapper mapper) : base(logger)
        {
            this._automatismoClientService = automatismoClientService;
			this._mapper = mapper;

		}

		/// <summary>
		///     swagger_summary_notificacionautomatismo_notificarajuste
		/// </summary>
		/// <remarks>swagger_remarks_notificacionautomatismo_notificarajuste</remarks>
		/// <param name="request"></param>
		/// <returns>swagger_returns_notificacionautomatismo_notificarajuste</returns>
		/// <response code="200">swagger_response_200_notificacionautomatismo_notificarajuste</response>
		/// <response code="400">swagger_response_400_notificacionautomatismo_notificarajuste</response>
		[HttpPost("NotificarAjuste")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AutomatismoResponse))]
		public async Task<IActionResult> NotificarAjuste([FromBody] NotificacionAjustesStockAutomatismoRequest request)
		{
			var response = new AutomatismoResponse();

            try
            {
				await LogRequest();
				return Ok(_automatismoClientService.NotificarAjuste(_mapper.Map(request)));
			}
			catch (Exception ex)
            {
                _logger.LogError(ex, $"NotificarAjuste > Error");
                response.SetError(ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        ///     swagger_summary_notificacionautomatismo_confirmarentrada
        /// </summary>
        /// <remarks>swagger_remarks_notificacionautomatismo_confirmarentrada</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_notificacionautomatismo_confirmarentrada</returns>
        /// <response code="200">swagger_response_200_notificacionautomatismo_confirmarentrada</response>
        /// <response code="400">swagger_response_400_notificacionautomatismo_confirmarentrada</response>
        [HttpPost("ConfirmarEntrada")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AutomatismoResponse))]
		public async Task<IActionResult> ConfirmarEntrada([FromBody] ConfirmacionEntradaStockAutomatismoRequest request)
		{
			var response = new AutomatismoResponse();

            try
            {
				await LogRequest();
				return Ok(_automatismoClientService.ConfirmarEntrada(_mapper.Map(request)));
			}
			catch (Exception ex)
            {
                _logger.LogError(ex, $"ConfirmarEntrada > Error");
                response.SetError(ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        ///     swagger_summary_notificacionautomatismo_confirmarsalida
        /// </summary>
        /// <remarks>swagger_remarks_notificacionautomatismo_confirmarsalida</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_notificacionautomatismo_confirmarsalida</returns>
        /// <response code="200">swagger_response_200_notificacionautomatismo_confirmarsalida</response>
        /// <response code="400">swagger_response_400_notificacionautomatismo_confirmarsalida</response>
        [HttpPost("ConfirmarSalida")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AutomatismoResponse))]
		public async Task<IActionResult> ConfirmarSalida([FromBody] ConfirmacionSalidaStockAutomatismoRequest request)
		{
			var response = new AutomatismoResponse();

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
    }
}
