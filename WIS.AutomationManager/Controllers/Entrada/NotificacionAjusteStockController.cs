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

    public class NotificacionAjusteStockController : AutomatismoBaseController
    {
        protected readonly INotificacionAjusteStockAutomatismoMapper _ajusteStockAutomatismoMapper;
        private readonly IValidationService _validationService;


        public NotificacionAjusteStockController(INotificacionAjusteStockAutomatismoMapper ajusteStockAutomatismoMapper,
            IAutomatismoService automatismoService,
            IAutomatismoEjecucionService automatismoEjecucionService,
            ILogger<NotificacionAjusteStockController> logger,
            IValidationService validationService) : base(logger, automatismoEjecucionService, automatismoService)
        {
            _ajusteStockAutomatismoMapper = ajusteStockAutomatismoMapper;
            _validationService = validationService;

        }

        /// <summary>
		///     swagger_summary_notificacionajustestock_send
		/// </summary>
		/// <remarks>swagger_remarks_notificacionajustestock_send</remarks>
		/// <param name="request"></param>
		/// <returns>swagger_returns_notificacionajustestock_send</returns>
		/// <response code="200">swagger_response_200_notificacionajustestock_send</response>
		/// <response code="400">swagger_response_400_notificacionajustestock_send</response>
        [HttpPost("Send")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Send([FromBody] NotificacionAjustesStockRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                var referencia = request.DsReferencia ?? "Notificación de ajustes de stock";
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

                        ejecucion = await this.AddEjecucion(null, CodigoInterfazAutomatismoDb.CD_INTERFAZ_NOTIF_AJUSTES, referencia, null, null);
                        return await ProcessError(ejecucion, request, new ValidationsResult() { Errors = errors });
                    }
                }

                ejecucion = await this.AddEjecucion(null, CodigoInterfazAutomatismoDb.CD_INTERFAZ_NOTIF_AJUSTES, referencia, null, loginName);

                var notificacion = this._ajusteStockAutomatismoMapper.Map(request);
                var response = await this._automatismoService.ProcesarNotificacionAjusteStock(request.Puesto, notificacion, ejecucion);

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
