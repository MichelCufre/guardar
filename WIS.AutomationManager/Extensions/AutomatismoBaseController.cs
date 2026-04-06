using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Enums;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.AutomationManager.Extensions
{
    /// <summary>
    /// 
    /// Nota: Los metodos de esta clase deben ser protegidos sino se rompe Swagger
    /// 
    /// </summary>
    public class AutomatismoBaseController : ControllerBase
    {
        private readonly string ProblemType = "https://tools.ietf.org/html/rfc7231";

        protected readonly IAutomatismoEjecucionService _ejecucionService;
        protected readonly IAutomatismoService _automatismoService;
        protected readonly ILogger _logger;

        public AutomatismoBaseController(ILogger logger,
            IAutomatismoEjecucionService ejecucionService,
            IAutomatismoService automatismoService)
        {
            _ejecucionService = ejecucionService;
            _logger = logger;
            _automatismoService = automatismoService;
        }

        protected virtual ObjectResult Problem400BadRequest(string detail, string title = null)
        {
            return Problem(detail, this.HttpContext.Request.Path, StatusCodes.Status400BadRequest, title, $"{ProblemType}#section-6.5.1");
        }

        protected virtual ObjectResult Problem404NotFound(string detail, string title = null)
        {
            return Problem(detail, this.HttpContext.Request.Path, StatusCodes.Status404NotFound, title, $"{ProblemType}#section-6.5.4");
        }

        protected virtual ObjectResult Problem409Conflict(string detail, string title = null)
        {
            return Problem(detail, this.HttpContext.Request.Path, StatusCodes.Status409Conflict, title, $"{ProblemType}#section-6.5.8");
        }

        protected virtual ObjectResult Problem500InternalServerError(string detail = null)
        {
            return Problem(detail, this.HttpContext.Request.Path, StatusCodes.Status500InternalServerError, "Internal Server Error", $"{ProblemType}#section-6.6");
        }

        protected virtual ObjectResult Problem500InternalServerError(ValidationsResult result)
        {
            return Problem(JsonConvert.SerializeObject(result.Errors), this.HttpContext.Request.Path, StatusCodes.Status500InternalServerError, "Internal Server Error", $"{ProblemType}#section-6.6");
        }

        protected virtual ObjectResult Problem401Unauthorized(string detail, string title = null)
        {
            return Problem(detail, this.HttpContext.Request.Path, StatusCodes.Status401Unauthorized, title, $"{ProblemType}#section-6.5.4");
        }

        protected async Task<AutomatismoEjecucion> AddEjecucion(int? nuAutomatismo, short cdInterfazExterna, string referencia, int? nuAutomatismoInterfaz, string identityUser)
        {
            return _ejecucionService.CrearEjecucion(nuAutomatismo, cdInterfazExterna, referencia, nuAutomatismoInterfaz, identityUser);
        }

        protected async Task<IActionResult> ProcessError(AutomatismoEjecucion ejecucion, object request, Exception ex)
        {
            var logId = Guid.NewGuid();
            ValidationsResult result = new ValidationsResult();

            string message = string.Format("Se ha producido un error no controlado. Por favor consulte con sistemas indicando el LogId {0}", logId);

            result.AddError(message);

            _logger.LogError(ex, message);

            return await this.ProcessError(ejecucion, request, result);
        }

		protected async Task<IActionResult> ProcessError(AutomatismoEjecucion ejecucion, object request, ValidationsResult result)
		{
			if (ejecucion != null)
            {
                ejecucion.Estado = EstadoEjecucion.PROCESADO_ERROR_API;
				ejecucion.AddData(request, result);
				_ejecucionService.Update(ejecucion);
            }

			return Problem400BadRequest(JsonConvert.SerializeObject(result.Errors), (ejecucion == null) ? "" : $"Error Interfaz Nro {ejecucion?.Id}");
		}

		protected async Task<IActionResult> ProcessOk(AutomatismoEjecucion ejecucion, object request, object response)
        {
            if (ejecucion != null)
            {
                ejecucion.Estado = EstadoEjecucion.PROCESADO_OK;
                ejecucion.AddData(request, response);

                _ejecucionService.Update(ejecucion);
            }

            return Ok(response);
        }

        protected string GetLoginName()
        {
            return HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
