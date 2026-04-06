using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Exceptions;

namespace WIS.WebApplication.Models
{
    public class ExceptionAsyncFilter : IAsyncExceptionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is InvalidUserException)
            {
                context.Result = new ObjectResult("Usuario no autorizado")
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };

                context.ExceptionHandled = true;
            }
            else if (context.Exception is UserNotAllowedException)
            {
                context.Result = new ObjectResult("Usuario no tiene permisos")
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };

                context.ExceptionHandled = true;
            }

            return Task.CompletedTask;
        }
    }
}
