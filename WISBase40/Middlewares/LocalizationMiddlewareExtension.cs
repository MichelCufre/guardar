using Microsoft.AspNetCore.Builder;

namespace WIS.WebApplication.Middlewares
{
    public static class LocalizationMiddlewareExtension
    {
        public static IApplicationBuilder UseLocalization(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LocalizationMiddleware>();
        }
    }
}