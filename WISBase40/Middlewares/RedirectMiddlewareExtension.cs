using Microsoft.AspNetCore.Builder;

namespace WIS.WebApplication.Middlewares
{
    public static class RedirectMiddlewareExtension
    {
        public static IApplicationBuilder UseRedirect(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RedirectMiddleware>();
        }
    }
}