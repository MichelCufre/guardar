using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WIS.Automation.Galys;

namespace WIS.GalysServices.Controllers.Salida
{
    [ApiController]
    [Route("[controller]")]
	public class ConfirmacionSalidaController : NotificacionControllerBase
	{
		public ConfirmacionSalidaController(IConfiguration config)
            : base(config)
        {
        }

        /// <summary>
        ///     swagger_summary_confirmacionsalida_confirmar
        /// </summary>
        /// <remarks>swagger_remarks_confirmacionsalida_confirmar</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_confirmacionsalida_confirmar/returns>
        [HttpPost]
        [Route("Confirmar")]
		public async Task<GalysResponse> Confirmar(ConfirmacionSalidaStockGalysRequest request)
		{
			return await SendNotification("/GalysNotification/ConfirmarSalida", request);
        }
    }
}
