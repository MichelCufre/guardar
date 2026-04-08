using Custom.Domain.DataModel.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WIS.Domain.DataModel;
using WIS.MiddlewareAPI.Dtos;
using WIS.MiddlewareAPI.Mappers;

namespace WIS.MiddlewareAPI.Controllers.Entrada
{
    [Route("[controller]")]
    [Produces("application/json")]
    public class ProductoController : MiddlewareControllerBase
    {
        public ProductoController(IUnitOfWorkFactory uowFactory, ILogger<ProductoController> logger)
            : base(uowFactory, logger) { }

        /// <summary>Encola la creacion o actualizacion de productos.</summary>
        /// <remarks>Recibe la estructura del ERP del cliente.</remarks>
        /// <response code="202">Solicitud encolada correctamente.</response>
        /// <response code="400">El cuerpo del request es invalido.</response>
        /// <response code="500">Error interno.</response>
        [HttpPost("CreateOrUpdate")]
        [ProducesResponseType(typeof(object), 202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateOrUpdate([FromBody] ErpProductosRequest request)
        {
            var wmsRequest = ErpMapper.ToWms(request);
            return Encolar(MiddlewareColaTipo.Producto, wmsRequest);
        }
    }
}
