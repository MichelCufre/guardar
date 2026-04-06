using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using WIS.Application.Setup;
using WIS.Exceptions;
using WIS.GridComponent.Execution;
using WIS.GridComponent.Execution.Serialization;
using WIS.Security;
using WIS.Session;

namespace WIS.Application.Invocation
{
    public class GridControllerInvocation : IGridControllerInvocation
    {
        protected readonly IGridActionResolver _gridActionResolver;
        protected readonly IApplicationSetupService _applicationSetupService;
        protected readonly ISecurityService _securityService;
        protected readonly ILogger<GridControllerInvocation> _logger;
        protected readonly ISessionAccessor _session;

        public GridControllerInvocation(
            IGridActionResolver gridActionResolver,
            IApplicationSetupService applicationSetupService,
            ILogger<GridControllerInvocation> logger,
            ISecurityService securityService, 
            ISessionAccessor session)
        {
            this._gridActionResolver = gridActionResolver;
            this._applicationSetupService = applicationSetupService;
            this._logger = logger;
            this._securityService = securityService;
            this._session = session;
        }

        public virtual IGridWrapper Invoke(IGridWrapper data, IGridController controller)
        {
            IGridWrapper response = new GridWrapper();

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

                response = this._gridActionResolver.InvokeAction(data, controller);

                response.SetSessionData(this._session.GetInnerDictionary());
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex, "Form - FormControllerInvocation - Invoke");

                response.SetError(ex.Message);
            }

            return response;
        }
    }
}
