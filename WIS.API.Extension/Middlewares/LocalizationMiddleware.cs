using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WIS.API.Extension.Middlewares
{
    public static class LocalizationMiddleware
    {
        public static void UseLocalizationMiddleware(this IApplicationBuilder app, IConfiguration configuration)
        {
            var supportedCultures = configuration.GetSection("Globalization:SupportedCultures").Get<List<string>>().Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Select(x => new CultureInfo(x)).ToList();
            string defaultCulture = configuration["Globalization:DefaultCulture"];

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture),

                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,

                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });
        }
    }
}
