using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using WIS.Automation.Galys;

namespace WIS.GalysServices.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    public class SalidaController : ControllerBase
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     swagger_summary_salida_add
        /// </summary>
        /// <remarks>swagger_remarks_salida_add</remarks>
        /// <param name="salida"></param>
        /// <returns>swagger_returns_salida_add</returns>
        [HttpPost]
        [Route("Add")]
		public GalysResponse Add(SalidaStockGalysRequest salida)
		{
			_logger.Debug($"Salida Stock: {JsonConvert.SerializeObject(salida)}");
            return new GalysResponse() { descError = "", resultado = 0 };
        }
    }
}
