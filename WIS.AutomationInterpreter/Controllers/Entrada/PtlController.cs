using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using WIS.AutomationInterpreter.Extensions;
using WIS.AutomationInterpreter.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Integracion.Dtos;
using WIS.Domain.Validation;

namespace WIS.AutomationInterpreter.Controllers.Entrada
{
    [ApiController]
    [AutomatismoAccessValidation]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PtlController : AutomatismoBaseController
    {
        private IPtlFactory _ptlFactory;

        public PtlController(
            ILogger<PtlController> logger,
            IPtlFactory ptlFactory) : base(logger)
        {
            _ptlFactory = ptlFactory;
        }


        [HttpPost("TurnLigthOnOrOff")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PtlCommandResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(PtlCommandResponse))]
        public IActionResult TurnLigthOnOrOff([FromBody] AutomatismoInterpreterRequest request)
        {
            var response = new PtlCommandResponse();

            try
            {
                return Ok(_ptlFactory.GetIntegrationService(request.IntegracionServicioConexion.InterfazExterna).TurnLigthOnOrOff(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"TurnLigthOnOrOff > Error");
                response.SetError(ex.Message);
                return NotFound(response);
            }
        }

        [HttpPost("StartOfOperation")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PtlCommandResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(PtlCommandResponse))]
        public IActionResult StartOfOperation([FromBody] AutomatismoInterpreterRequest request)
        {
            var response = new PtlCommandResponse();

            try
            {
                return Ok(_ptlFactory.GetIntegrationService(request.IntegracionServicioConexion.InterfazExterna).StartOfOperation(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"StartOfOperation > Error");
                response.SetError(ex.Message);
                return NotFound(response);
            }
        }


        [HttpPost("ResetOfOperation")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PtlCommandResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(PtlCommandResponse))]
        public IActionResult ResetOfOperation([FromBody] AutomatismoInterpreterRequest request)
        {
            var response = new PtlCommandResponse();

            try
            {
                return Ok(_ptlFactory.GetIntegrationService(request.IntegracionServicioConexion.InterfazExterna).ResetOfOperation(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ResetOfOperation > Error");
                response.SetError(ex.Message);
                return NotFound(response);
            }
        }
    }
}
