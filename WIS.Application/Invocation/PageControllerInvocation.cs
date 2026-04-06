using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Setup;
using WIS.Exceptions;
using WIS.PageComponent.Execution;
using WIS.PageComponent.Execution.Serialization;
using WIS.Security;
using WIS.Session;
using WIS.TrafficOfficer;

namespace WIS.Application.Invocation
{
    public class PageControllerInvocation : IPageControllerInvocation
    {
        protected readonly IPageActionResolver _pageActionResolver;
        protected readonly IApplicationSetupService _applicationSetupService;
        protected readonly ITrafficOfficerService _trafficOfficerService;
        protected readonly ISecurityService _securityService;
        protected readonly ILogger<PageControllerInvocation> _logger;
        protected readonly ISessionAccessor _session;

        public PageControllerInvocation(
            IPageActionResolver pageActionResolver,
            IApplicationSetupService applicationSetupService,
            ITrafficOfficerService trafficOfficerService,
            ISecurityService securityService,
            ILogger<PageControllerInvocation> logger, 
            ISessionAccessor session)
        {
            this._pageActionResolver = pageActionResolver;
            this._applicationSetupService = applicationSetupService;
            this._trafficOfficerService = trafficOfficerService;
            this._securityService = securityService;
            this._logger = logger;
            this._session = session;
        }

        public virtual IPageWrapper Invoke(IPageWrapper data, IPageController controller)
        {
            IPageWrapper response = new PageWrapper();

            try
            {
                this._applicationSetupService.SetupServices(new ApplicationSetupInfo
                {
                    Application = data.Application,
                    Predio = data.Predio,
                    User = data.User,
                    Token = data.PageToken,
                    Session = data.GetSessionData()
                });

                if (!this._securityService.CanUserAccessApplication())
                    throw new UserNotAllowedException("General_Sec0_Error_AccessNotAllowed");

                if (data.Action == PageAction.Load)
                    data.PageToken = this._trafficOfficerService.CreateToken();

                response = this._pageActionResolver.InvokeAction(data, controller);

                response.SetSessionData(this._session.GetInnerDictionary());

                if (data.Action == PageAction.Unload)
                {
                    this._trafficOfficerService.ClearToken();
                    response.PageToken = null;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Page - PageControllerInvocation - Invoke");

                response.SetError(ex.Message);
            }

            return response;
        }
    }
}
