using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using WIS.Automation.Galys;

namespace WIS.GalysServices.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    public class CodigoBarraController : ControllerBase
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
		///     swagger_summary_codigobarra_add
		/// </summary>
		/// <remarks>swagger_remarks_codigobarra_add</remarks>
		/// <param name="codigoBarra"></param>
		/// <returns>swagger_returns_codigobarra_add</returns>
        [HttpPost]
        [Route("Add")]
		public GalysResponse Add(CodigoBarraGalysRequest codigoBarra)
		{
			_logger.Debug($"Codigo barra: {JsonConvert.SerializeObject(codigoBarra)}");

            return new GalysResponse() { descError = "", resultado = 0 };
        }
    }
}
