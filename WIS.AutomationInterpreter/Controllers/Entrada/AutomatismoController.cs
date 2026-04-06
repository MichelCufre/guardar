using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using WIS.Automation;
using WIS.AutomationInterpreter.Extensions;
using WIS.AutomationInterpreter.Interfaces;
using WIS.Domain.Integracion.Dtos;
using WIS.Domain.Validation;

namespace WIS.AutomationInterpreter.Controllers.Entrada
{
    [ApiController]
    [AutomatismoAccessValidation]
    [Produces("application/json")]
    [Route("[controller]")]
    public class AutomatismoController : AutomatismoBaseController
    {
        private IAutomatismoFactory _automatismoFactory;

        public AutomatismoController(
            ILogger<AutomatismoController> logger,
            IAutomatismoFactory automatismoFacctory) : base(logger)
        {
            this._automatismoFactory = automatismoFacctory;
        }


        /// <summary>
        ///     swagger_summary_automationinterpreter_sendproductos
        /// </summary>
        /// <remarks>swagger_remarks_automationinterpreter_sendproductos</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_automationinterpreter_sendproductos</returns>
        /// <response code="200">swagger_response_200_automationinterpreter_sendproductos</response>
        /// <response code="400">swagger_response_400_automationinterpreter_sendproductos</response>
        [HttpPost("SendProductos")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AutomatismoResponse))]
        public IActionResult SendProductos([FromBody] AutomatismoInterpreterRequest request)
        {
            var response = new AutomatismoResponse();

            try
            {
                return Ok(_automatismoFactory.GetIntegrationService(request.IntegracionServicioConexion.InterfazExterna).SendProductos(request));
            }
            catch (Exception ex)
            {
                response.SetError(ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        ///     swagger_summary_automationinterpreter_sendcodigosbarras
        /// </summary>
        /// <remarks>swagger_remarks_automationinterpreter_sendcodigosbarras</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_automationinterpreter_sendcodigosbarras</returns>
        /// <response code="200">swagger_response_200_automationinterpreter_sendcodigosbarras</response>
        /// <response code="400">swagger_response_400_automationinterpreter_sendcodigosbarras</response>
        [HttpPost("SendCodigosBarras")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AutomatismoResponse))]
        public IActionResult SendCodigosBarras([FromBody] AutomatismoInterpreterRequest request)
        {
            var response = new AutomatismoResponse();

            try
            {
                return Ok(_automatismoFactory.GetIntegrationService(request.IntegracionServicioConexion.InterfazExterna).SendCodigosBarras(request));
            }
            catch (Exception ex)
            {
                response.SetError(ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        ///     swagger_summary_automationinterpreter_sendsalida
        /// </summary>
        /// <remarks>swagger_remarks_automationinterpreter_sendsalida</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_automationinterpreter_sendsalida</returns>
        /// <response code="200">swagger_response_200_automationinterpreter_sendsalida</response>
        /// <response code="400">swagger_response_400_automationinterpreter_sendsalida</response>
        [HttpPost("SendSalida")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AutomatismoResponse))]
        public IActionResult SendSalida([FromBody] AutomatismoInterpreterRequest request)
        {
            var response = new AutomatismoResponse();

            try
            {
                return Ok(_automatismoFactory.GetIntegrationService(request.IntegracionServicioConexion.InterfazExterna).SendSalida(request));
            }
            catch (Exception ex)
            {
                response.SetError(ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        ///     swagger_summary_automationinterpreter_sendentrada
        /// </summary>
        /// <remarks>swagger_remarks_automationinterpreter_sendentrada</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_automationinterpreter_sendentrada</returns>
        /// <response code="200">swagger_response_200_automationinterpreter_sendentrada</response>
        /// <response code="400">swagger_response_400_automationinterpreter_sendentrada</response>
        [HttpPost("SendEntrada")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AutomatismoResponse))]
        public IActionResult SendEntrada([FromBody] AutomatismoInterpreterRequest request)
        {

            var response = new AutomatismoResponse();

            try
            {
                return Ok(_automatismoFactory.GetIntegrationService(request.IntegracionServicioConexion.InterfazExterna).SendEntrada(request));
            }
            catch (Exception ex)
            {
                response.SetError(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
