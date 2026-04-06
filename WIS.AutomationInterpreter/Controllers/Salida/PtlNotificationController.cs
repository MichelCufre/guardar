using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.AutomationInterpreter.Extensions;
using WIS.AutomationInterpreter.Services;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;

namespace WIS.AutomationInterpreter.Controllers.Salida
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PtlNotificationController : AutomatismoBaseController
    {
        private AutomatismoClientService _automatismoClientService;

        public PtlNotificationController(
            ILogger<PtlNotificationController> logger,
            AutomatismoClientService automtismoClientService) : base(logger)
        {
            _automatismoClientService = automtismoClientService;
        }

        [HttpPost("ConfirmCommand")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PtlCommandResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(PtlCommandResponse))]
		public async Task<IActionResult> ConfirmCommand([FromBody] PtlCommandConfirmRequest request)
		{
			var response = new PtlCommandResponse();

			await LogRequest();

			try
            {
                if (request.Color == "1")
                {
                    request.CommandType = PtlTipoComandoDb.Cancelacion;
                }
                else
                {
                    request.CommandType = PtlTipoComandoDb.Confirmacion;
                }

                return Ok(_automatismoClientService.ConfirmCommand(request));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ConfirmCommand > Error");
                response.SetError(ex.Message);
                return NotFound(response);
            }
        }

        [HttpPost("ConfirmCommandList")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PtlCommandResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(PtlCommandResponse))]
		public async Task<IActionResult> ConfirmCommandList([FromBody] List<PtlCommandConfirmRequest> requestList)
		{
			var response = new PtlCommandResponse();

			await LogRequest();

			try
            {
                foreach (var request in requestList)
                {
                    if (request.Color == "1")
                    {
                        request.CommandType = PtlTipoComandoDb.Cancelacion;
                    }
                    else
                    {
                        request.CommandType = PtlTipoComandoDb.Confirmacion;
                    }

                    response = _automatismoClientService.ConfirmCommand(request);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ConfirmCommandList > Error");
                response.SetError(ex.Message);
                return NotFound(response);
            }
        }
    }
}
