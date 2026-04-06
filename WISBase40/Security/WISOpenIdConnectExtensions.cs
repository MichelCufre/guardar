using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace WIS.WebApplication.Security
{
    public static class WISOpenIdConnectExtensions
    {
        public static AuthenticationBuilder AddWISOpenIdConnect(this AuthenticationBuilder builder)
        {
            return builder.AddWISOpenIdConnect("OpenIdConnect", delegate
            {
            });
        }

        public static AuthenticationBuilder AddWISOpenIdConnect(this AuthenticationBuilder builder, Action<OpenIdConnectOptions> configureOptions)
        {
            return builder.AddWISOpenIdConnect("OpenIdConnect", configureOptions);
        }

        public static AuthenticationBuilder AddWISOpenIdConnect(this AuthenticationBuilder builder, string authenticationScheme, Action<OpenIdConnectOptions> configureOptions)
        {
            return builder.AddWISOpenIdConnect(authenticationScheme, OpenIdConnectDefaults.DisplayName, configureOptions);
        }

        public static AuthenticationBuilder AddWISOpenIdConnect(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<OpenIdConnectOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());
            return builder.AddRemoteScheme<OpenIdConnectOptions, WISOpenIdConnectHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
