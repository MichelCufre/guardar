using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Tracking.Validation;
using WIS.WMSTrackingAPI.Extensions;
using WIS.WMSTrackingAPI.Models.Mappers.Interfaces;

namespace WIS.WMSTrackingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PedidoPuntoEntregaController : ControllerBaseExtension
    {
        private readonly IPedidoService _pedidoService;
        private readonly IPuntoEntregaMapper _puntoEntregaMapper;
        private readonly ILogger<PedidoPuntoEntregaController> _logger;

        public PedidoPuntoEntregaController(IPedidoService pedidoService, IPuntoEntregaMapper puntoEntregaMapper, ILogger<PedidoPuntoEntregaController> logger)
        {
            this._logger = logger;
            this._pedidoService = pedidoService;
            this._puntoEntregaMapper = puntoEntregaMapper;
        }

        /// <summary>
        ///     swagger_summary_pedidopuntoentrega_updatepedidospuntoentrega
        /// </summary>
        /// <remarks>swagger_remarks_pedidopuntoentrega_updatepedidospuntoentrega</remarks>
        /// <param name="request"></param>
        [HttpPost("UpdatePedidosPuntoEntrega")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePedidosPuntoEntrega([FromBody] PuntoEntregaAgentesRequest request)
        {
            try
            {
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var puntoEntrega = _puntoEntregaMapper.Map(request);
                TrackingValidationResult result = await _pedidoService.ActualizarPedidosPuntoEntrega(puntoEntrega, loginName);

                if (result.HasError())
                    return BadRequest(JsonConvert.SerializeObject(result));

                return Ok();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }
    }
}
