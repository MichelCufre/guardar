using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WIS.Automation.Galys;

namespace WIS.GalysServices.Controllers.Salida
{
    [ApiController]
    [Route("[controller]")]
	public class ConfirmacionEntradaController : NotificacionControllerBase
	{
		public ConfirmacionEntradaController(IConfiguration config)
            : base(config)
        {
        }

        /// <summary>
        ///     swagger_summary_confirmacionentrada_confirmar
        /// </summary>
        /// <remarks>swagger_remarks_confirmacionentrada_confirmar</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_confirmacionentrada_confirmar/returns>
        [HttpPost]
        [Route("Confirmar")]
		public async Task<GalysResponse> Confirmar(ConfirmacionEntradaStockGalysRequest request)
		{
			return await SendNotification("/GalysNotification/ConfirmarEntradas", request);
        }
    }
}
