using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NLog;
using System;
using System.Linq;
using WIS.Exceptions;
using WIS.TrafficOfficer;
using WIS.WebApplication.Models;

namespace WIS.WebApplication.ActionFilters
{
    public class CheckAuthorization : ActionFilterAttribute
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var trafficOfficer = filterContext.HttpContext.RequestServices.GetService(typeof(ITrafficOfficerFrontendService)) as ITrafficOfficerFrontendService;
            var preventSessionUpdateHeader = "PreventSessionUpdate".ToLower();

            if (filterContext.HttpContext.Request.Headers.Any(h => h.Key.ToLower() == preventSessionUpdateHeader))
            {
                var header = filterContext.HttpContext.Request.Headers.First(h => h.Key.ToLower() == preventSessionUpdateHeader);
                if (header.Value.Any(v => v.ToLower() == "true"))
                    return;
            }

            try
            {
                if (!string.IsNullOrEmpty(trafficOfficer.SessionToken))
                    trafficOfficer.UpdateSessionActivity(default).Wait();
                else
                    throw new ValidationFailedException("TOKEN NO ASIGNADO");

                if (filterContext.ActionArguments.Any(a => a.Value is ServerRequest))
                {
                    var serverRequest = (ServerRequest)filterContext.ActionArguments.FirstOrDefault(a => a.Value is ServerRequest).Value;

                    if (!string.IsNullOrEmpty(serverRequest.PageToken))
                    {
                        trafficOfficer.UpdateThreadOperationActivity(serverRequest.PageToken, default).Wait();
                    }
                }
            }
            catch (ExpiredPageTokenException ex)
            {
                this._logger.Error(ex, ex.Message);
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "Index" }
                    });
                return;
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                filterContext.Result = GetAuthErrorActionResult(filterContext);
                return;
            }
        }

        private ActionResult GetAuthErrorActionResult(ActionExecutingContext filterContext)
        {
            var isAjaxCall = false;
            var ajaxHeader = "X-Requested-With".ToLower();

            if (filterContext.HttpContext.Request.Headers.Any(h => h.Key.ToLower() == ajaxHeader))
            {
                var header = filterContext.HttpContext.Request.Headers.First(h => h.Key.ToLower() == ajaxHeader);
                isAjaxCall = header.Value.Any(v => v.ToLower() == "XMLHttpRequest".ToLower());
            }

            if (isAjaxCall)
            {
                filterContext.HttpContext.Response.Headers.Remove("Set-Cookie");
                return new UnauthorizedResult();
            }

            return new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Security" },
                    { "action", "Logout" }
                });
        }
    }
}
