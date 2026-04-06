using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using WIS.Automation.Galys;

namespace WIS.GalysServices.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    public class ProductoController : ControllerBase
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     swagger_summary_producto_add
        /// </summary>
        /// <remarks>swagger_remarks_producto_add</remarks>
        /// <param name="producto"></param>
        /// <returns>swagger_returns_producto_add</returns>
        [HttpPost]
        [Route("Add")]
		public GalysResponse Add(ProductoGalysRequest producto)
		{
			_logger.Debug($"Producto: {JsonConvert.SerializeObject(producto)}");
            return new GalysResponse() { descError = "", resultado = 0 };
        }
    }
}
