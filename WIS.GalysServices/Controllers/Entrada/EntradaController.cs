using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using WIS.Automation.Galys;

namespace WIS.GalysServices.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    public class EntradaController : ControllerBase
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     swagger_summary_entrada_add
        /// </summary>
        /// <remarks>swagger_remarks_entrada_add</remarks>
        /// <param name="entrada"></param>
        /// <returns>swagger_returns_entrada_add</returns>
        [HttpPost]
        [Route("Add")]
		public GalysResponse Add(EntradaStockGalysRequest entrada)
		{
			_logger.Debug($"Entrada Stock: {JsonConvert.SerializeObject(entrada)}");

            return new GalysResponse() { descError = "", resultado = 0 };
        }
    }
}
