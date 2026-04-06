using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.TaskQueue.Extensions
{
    /// <summary>
    /// 
    /// ]Nota: Los metodos de esta clase deben ser protegidos sino se rompe swagger
    /// 
    /// </summary>
    public abstract class ControllerBaseExtension : ControllerBase
    {
        private readonly string ProblemType = "https://tools.ietf.org/html/rfc7231";

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
        protected virtual ObjectResult Problem401Unauthorized(string detail, string title = null)
        {
            return Problem(detail, this.HttpContext.Request.Path, StatusCodes.Status401Unauthorized, title, $"{ProblemType}#section-6.5.4");
        }
    }
}
