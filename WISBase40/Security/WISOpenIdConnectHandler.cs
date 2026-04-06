using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WIS.WebApplication.Security
{
    public class WISOpenIdConnectHandler : OpenIdConnectHandler
    {
        public WISOpenIdConnectHandler(IOptionsMonitor<OpenIdConnectOptions> options, ILoggerFactory logger, HtmlEncoder htmlEncoder, UrlEncoder encoder, ISystemClock clock) : base(options, logger, htmlEncoder, encoder, clock)
        {
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var oldNonces = Request.Cookies.Where(kvp => kvp.Key.StartsWith(base.Options.NonceCookie.Name));
            if (oldNonces.Any())
            {
                CookieOptions options = base.Options.NonceCookie.Build(base.Context, base.Clock.UtcNow);
                foreach (KeyValuePair<string, string> oldNonce in oldNonces)
                {
                    Response.Cookies.Delete(oldNonce.Key, options);
                }
            }

            if (!Context.User.Identity.IsAuthenticated)
            {
                var isAjaxCall = false;
                var ajaxHeader = "X-Requested-With".ToLower();

                if (Request.Headers.Any(h => h.Key.ToLower() == ajaxHeader))
                {
                    var header = Request.Headers.First(h => h.Key.ToLower() == ajaxHeader);
                    isAjaxCall = header.Value.Any(v => v.ToLower() == "XMLHttpRequest".ToLower());
                }

                if (isAjaxCall)
                {
                    Response.StatusCode = 401;
                    Response.Headers.Remove("Set-Cookie");
                    return Task.CompletedTask;
                }
            }

            return base.HandleChallengeAsync(properties);
        }
    }
}
