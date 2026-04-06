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
    public class ProductoController : AutomatismoBaseController
    {
        public ProductoController(IAutomatismoService automatismoService,
            IAutomatismoEjecucionService automatismoEjecucionService,
            ILogger<ProductoController> logger) : base(logger, automatismoEjecucionService, automatismoService)
        {
        }
        /// <summary>
        ///     swagger_summary_producto_update
        /// </summary>
        /// <remarks>swagger_remarks_producto_update</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_producto_update</returns>
        /// <response code="200">swagger_response_200_producto_update</response>
        /// <response code="400">swagger_response_400_producto_update</response>
        [HttpPost("Update")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CodigoInterfazAutomatismoDb.CD_INTERFAZ_PRODUCTOS, ParamManager.IE_1500_HABILITADA, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Update([FromBody] ProductosAutomatismoRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                var referencia = request.DsReferencia ?? "Manejo de productos automatismo";
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                ejecucion = await this.AddEjecucion(null, CodigoInterfazAutomatismoDb.CD_INTERFAZ_PRODUCTOS, referencia, null, loginName);

                request.Productos.ForEach(p => { p.TipoOperacion = TipoOperacionDb.Sobrescritura; });

                var response = await this._automatismoService.NotificarProductos(request, ejecucion);

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
