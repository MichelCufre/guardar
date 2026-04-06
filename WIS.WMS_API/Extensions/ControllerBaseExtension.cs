using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Exceptions;

namespace WIS.WMS_API.Extensions
{
    /// <summary>
    /// 
    /// ]Nota: Los metodos de esta clase deben ser protegidos sino se rompe swagger
    /// 
    /// </summary>
    public abstract class ControllerBaseExtension : ControllerBase
    {
        private readonly string ProblemType = "https://tools.ietf.org/html/rfc7231";

        protected virtual ObjectResult Problem400BadRequest(string detail, string title, long nuInterfazEjecucion)
        {
            var extensions = new Dictionary<string, object>()
            {
                ["NumeroInterfazEjecucion"] = nuInterfazEjecucion
            };

            return Problem400BadRequest(detail, title, extensions);
        }

        protected virtual ObjectResult Problem400BadRequest(string detail, string title = null, Dictionary<string, object> extensions = null)
        {
            var problem = Problem(detail, this.HttpContext.Request.Path, StatusCodes.Status400BadRequest, title, $"{ProblemType}#section-6.5.1");

            if (problem.Value is ProblemDetails problemDetails && extensions != null && extensions.Any())
                problemDetails.Extensions = extensions;

            return problem;
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

        protected virtual ObjectResult Problem401Unauthorized(string detail, string title = null)
        {
            return Problem(detail, this.HttpContext.Request.Path, StatusCodes.Status401Unauthorized, title, $"{ProblemType}#section-6.5.4");
        }

        protected virtual string GetLoginName()
        {
            return HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        public virtual async Task<T> GetRequest<T>()
        {
            string body;
            HttpContext.Request.EnableBuffering();
            HttpContext.Request.Body.Position = 0;

            using (var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                HttpContext.Request.Body.Position = 0;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch (JsonSerializationException ex)
            {
                throw new ValidationFailedException(ex.Message);
            }
            catch (JsonReaderException ex)
            {
                throw new ValidationFailedException(ex.Message);
            }
        }
    }
}
