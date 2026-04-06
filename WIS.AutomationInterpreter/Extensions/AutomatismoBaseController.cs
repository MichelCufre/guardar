using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.AutomationInterpreter.Extensions
{
    public class AutomatismoBaseController : ControllerBase
    {
        private readonly string ProblemType = "https://tools.ietf.org/html/rfc7231";

        protected readonly ILogger _logger;

        public AutomatismoBaseController(ILogger logger)
        {
            _logger = logger;
        }


        protected string GetLoginName()
        {
            return HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
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

		protected virtual async Task LogRequest()
		{
			var payload = string.Empty;

			Request.Body.Position = 0;

			using (var sr = new StreamReader(Request.Body, Encoding.UTF8))
			{
				payload = await sr.ReadToEndAsync();
			}

			_logger.LogDebug(payload);
		}
	}
}
