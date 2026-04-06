using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WIS.Automation.Galys;

namespace WIS.GalysServices.Controllers.Salida
{
    [ApiController]
    [Route("[controller]")]
	public class NotificacionAjusteController : NotificacionControllerBase
	{
		public NotificacionAjusteController(IConfiguration config)
            : base(config)
        {
        }

        /// <summary>
        ///     swagger_summary_notificacionajuste_notificar
        /// </summary>
        /// <remarks>swagger_remarks_notificacionajuste_notificar</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_notificacionajuste_notificar</returns>
        [HttpPost]
        [Route("Notificar")]
		public async Task<GalysResponse> Notificar(NotificacionAjusteStockGalysRequest request)
		{
			return await SendNotification("/GalysNotification/NotificarAjustes", request);
        }
    }
}
