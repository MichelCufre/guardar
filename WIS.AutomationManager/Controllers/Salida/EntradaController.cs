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
    public class EntradaController : AutomatismoBaseController
    {
        public EntradaController(IAutomatismoService automatismoService,
            IAutomatismoEjecucionService automatismoEjecucionService,
            ILogger<ProductoController> logger) : base(logger, automatismoEjecucionService, automatismoService)
        {
        }

        /// <summary>
		///     swagger_summary_entrada_entradastock
		/// </summary>
		/// <remarks>swagger_remarks_entrada_entradastock</remarks>
		/// <param name="request"></param>
		/// <returns>swagger_returns_entrada_entradastock</returns>
		/// <response code="200">swagger_response_200_entrada_entradastock</response>
		/// <response code="400">swagger_response_400_entrada_entradastock</response>
        [HttpPost("EntradaStock")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS, ParamManager.IE_1600_HABILITADA, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> EntradaStock([FromBody] EntradaStockAutomatismoRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                var referencia = request.DsReferencia ?? "Manejo de entradas automatismo";
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                ejecucion = await this.AddEjecucion(null, CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS, referencia, null, loginName);
                request.Ejecucion = ejecucion.Id;

                var response = await this._automatismoService.NotificarEntrada(request, ejecucion);

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
