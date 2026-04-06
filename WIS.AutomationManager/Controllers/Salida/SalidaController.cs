using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using WIS.Automation;
using WIS.AutomationManager.Extensions;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Validation;

namespace WIS.AutomationManager.Controllers.Salida
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class SalidaController : AutomatismoBaseController
    {
        public SalidaController(IAutomatismoService automatismoService,
            IAutomatismoEjecucionService automatismoEjecucionService,
            ILogger<SalidaController> logger) : base(logger, automatismoEjecucionService, automatismoService)
        {
        }

        /// <summary>
        ///     swagger_summary_salida_salidastock
        /// </summary>
        /// <remarks>swagger_remarks_salida_salidastock</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_salida_salidastock</returns>
        /// <response code="200">swagger_response_200_salida_salidastock</response>
        /// <response code="400">swagger_response_400_salida_salidastock</response>
        [HttpPost("SalidaStock")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CodigoInterfazAutomatismoDb.CD_INTERFAZ_SALIDA, ParamManager.IE_1700_HABILITADA, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> SalidaStock([FromBody] SalidaStockAutomatismoRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                var referencia = request.DsReferencia ?? "Manejo de salidas automatismo";
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                ejecucion = await this.AddEjecucion(null, CodigoInterfazAutomatismoDb.CD_INTERFAZ_SALIDA, referencia, null, loginName);
                request.Ejecucion = ejecucion.Id;

                var response = await this._automatismoService.NotificarSalida(request, ejecucion);

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
