using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;

namespace WIS.AutomationGateway.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, Logger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        context.Response.StatusCode = (int)GetErrorCode(contextFeature.Error);
                        context.Response.ContentType = "application/json";

                        var logId = Guid.NewGuid();
                        logger.Error(GetErrorDetails(logId, contextFeature.Error));

                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            LogID = logId,
                            Message = "Se ha producido un error no controlado. Por favor consulte con sistemas indicando el LogId"
                        }));
                    }
                });
            });
        }

        private static HttpStatusCode GetErrorCode(Exception e)
        {
            switch (e)
            {
                case ValidationException _:
                    return HttpStatusCode.BadRequest;
                case AuthenticationException _:
                    return HttpStatusCode.Forbidden;
                case NotImplementedException _:
                    return HttpStatusCode.NotImplemented;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        }

        private static string GetErrorDetails(Guid logId, Exception ex)
        {
            var details = new List<string>()
            {
                $"LogId: {logId}",
                $"Error type: {ex.GetType().Name}",
                $"Error message: { ex.Message}",
                $"Stack trace: { ex.StackTrace}"
            };

            var inner = ex.InnerException;

            while (inner != null)
            {
                details.Add($"Inner error type: {inner.GetType().Name}");
                details.Add($"Inner error message: { inner.Message}");
                details.Add($"Inner error stack trace: { inner.StackTrace}");
                inner = inner.InnerException;
            }

            return string.Join(". " + Environment.NewLine, details);
        }
    }
}
