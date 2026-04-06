using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using WIS.Security;
using WIS.TrackingAPI.Domain.DataModel;

namespace WIS.TrackingAPI.Filters
{
    public class WISActionFilter : IActionFilter
    {
        private readonly IIdentityService _identity;

        public WISActionFilter(IIdentityService identity)
        {
            _identity = identity;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var loginName = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(loginName))
            {
                var actionDescriptor = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor;
                var controllerName = actionDescriptor.ControllerName;
                var actionName = actionDescriptor.ActionName;
                var manager = (IIdentityServiceManager)_identity;
                var application = $"{controllerName}.{actionName}";
                var user = new BasicUserData
                {
                    UserId = -2, //tracking
                    Language = "es"
                };

                application = application.Substring(0, Math.Min(200, application.Length));
                manager.SetUser(user, application, "S/D");
            }
        }
    }
}
