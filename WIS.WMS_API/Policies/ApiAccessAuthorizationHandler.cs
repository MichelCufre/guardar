using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WIS.WMS_API.Policies
{
    public class ApiAccessAuthorizationHandler : AuthorizationHandler<ApiAccessRequirement>
    {
        private readonly IConfiguration _configuration;

        public ApiAccessAuthorizationHandler(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiAccessRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
                return;

            string scope = this._configuration.GetValue<string>("AuthSettings:AccessScope");

            var requestScope = context.User.FindFirstValue(OpenIddictConstants.Claims.Scope);

            if (string.IsNullOrEmpty(requestScope))
                return;

            string[] scopes = requestScope.Split(' ');

            if (scopes.Contains(scope))
                context.Succeed(requirement);
        }
    }
}
