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
    public class ConfirmacionEntradaController : AutomatismoBaseController
    {
        protected readonly IConfirmacionEntradaAutomatismoMapper _entradaStockAutomatismoMapper;
        private readonly IValidationService _validationService;

        public ConfirmacionEntradaController(IConfirmacionEntradaAutomatismoMapper entradaStockAutomatismoMapper,
            IAutomatismoService automatismoService,
            IAutomatismoEjecucionService automatismoEjecucionService,
            ILogger<ConfirmacionEntradaController> logger,
            IValidationService validationService) : base(logger, automatismoEjecucionService, automatismoService)
        {
            _entradaStockAutomatismoMapper = entradaStockAutomatismoMapper;
            _validationService = validationService;
        }

        /// <summary>
		///     swagger_summary_confirmacionentrada_send
		/// </summary>
		/// <remarks>swagger_remarks_confirmacionentrada_send</remarks>
		/// <param name="request"></param>
		/// <returns>swagger_returns_confirmacionentrada_send</returns>
		/// <response code="200">swagger_response_200_confirmacionentrada_send</response>
		/// <response code="400">swagger_response_400_confirmacionentrada_send</response>
        [HttpPost("Send")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Send([FromBody] ConfirmacionEntradaStockRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                var referencia = request.DsReferencia ?? "Confirmación de entrada de stock";
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

                        ejecucion = await this.AddEjecucion(null, CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_ENTRADAS, referencia, null, null);
                        return await ProcessError(ejecucion, request, new ValidationsResult() { Errors = errors });

                    }
                }

                ejecucion = await this.AddEjecucion(null, CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_ENTRADAS, referencia, null, loginName);

                var confirmacion = this._entradaStockAutomatismoMapper.Map(request);
                var response = await this._automatismoService.ProcesarConfirmacionEntrada(request.Puesto, confirmacion, ejecucion);

                if (response?.HasError() ?? false)
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
