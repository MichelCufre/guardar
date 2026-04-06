using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Automation;
using WIS.AutomationManager.Extensions;
using WIS.AutomationManager.Interfaces;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Exceptions;

namespace WIS.AutomationManager.Controllers.Entrada
{
    [ApiController]
    [AutomatismoAccessValidation]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ConfirmacionMovimientoController : AutomatismoBaseController
    {
        private readonly IValidationService _validationService;

        public ConfirmacionMovimientoController(
            IAutomatismoService automatismoService,
            IAutomatismoEjecucionService automatismoEjecucionService,
            ILogger<ConfirmacionMovimientoController> logger,
            IValidationService validationService) : base(logger, automatismoEjecucionService, automatismoService)
        {
            _validationService = validationService;
        }

        /// <summary>
		///     swagger_summary_ConfirmacionMovimiento_send
		/// </summary>
		/// <remarks>swagger_remarks_ConfirmacionMovimiento_send</remarks>
		/// <param name="request"></param>
		/// <returns>swagger_returns_ConfirmacionMovimiento_send</returns>
		/// <response code="200">swagger_response_200_ConfirmacionMovimiento_send</response>
		/// <response code="400">swagger_response_400_ConfirmacionMovimiento_send</response>
        [HttpPost("Send")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Send([FromBody] ConfirmacionMovimientoStockRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                var referencia = request.DsReferencia ?? "Movimiento de stock";
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (request.Usuario != null)
                {
                    if (_automatismoService.IsValidUser(request.Usuario, request.Puesto))
                        loginName = request.Usuario.LoginName;
                    else
                    {
                        List<ValidationsError> errors = new List<ValidationsError>();
                        var error = new Error("WMSAPI_msg_Error_UsuarioPuestoNoValido", request.Usuario.LoginName, request.Puesto);
                        var errorTraslation = _validationService.Translate(error);
                        errors.Add(new ValidationsError(1, false, new List<string>() { errorTraslation }));

                        ejecucion = await this.AddEjecucion(null, CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_MOVIMIENTO, referencia, null, null);
                        return await ProcessError(ejecucion, request, new ValidationsResult() { Errors = errors });
                    }
                }

                ejecucion = await this.AddEjecucion(null, CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_MOVIMIENTO, referencia, null, loginName);

                var response = await this._automatismoService.ProcesarConfirmacionMovimiento(request.Puesto, request, ejecucion);

                if (response.HasError())
                    return await ProcessError(ejecucion, request, response);

                return await ProcessOk(ejecucion, request, response);

            }
            catch (Exception ex)
            {
                return await ProcessError(ejecucion, request, ex);
            }
        }
    }
}
