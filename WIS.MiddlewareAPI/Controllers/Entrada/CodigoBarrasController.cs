using Custom.Domain.DataModel.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WIS.Domain.DataModel;

namespace WIS.MiddlewareAPI.Controllers.Entrada
{
    [Route("[controller]")]
    [Produces("application/json")]
    public class CodigoBarrasController : MiddlewareControllerBase
    {
        public CodigoBarrasController(IUnitOfWorkFactory uowFactory, ILogger<CodigoBarrasController> logger)
            : base(uowFactory, logger) { }

        /// <summary>Encola la creacion o actualizacion de codigos de barras.</summary>
        /// <remarks>Recibe el payload crudo del cliente para encolar y traducir en el batch.</remarks>
        /// <response code="202">Solicitud encolada correctamente.</response>
        /// <response code="400">El cuerpo del request es invalido.</response>
        /// <response code="500">Error interno.</response>
        [HttpPost("CreateUpdateOrDelete")]
        [ProducesResponseType(typeof(object), 202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateOrUpdate([FromBody] string payload)
        {
            return Encolar(MiddlewareColaTipo.CodigoBarras, payload);
        }
    }
}
