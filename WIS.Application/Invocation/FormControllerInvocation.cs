using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using WIS.Application.Setup;
using WIS.Exceptions;
using WIS.FormComponent.Execution;
using WIS.FormComponent.Execution.Serialization;
using WIS.Security;
using WIS.Session;

namespace WIS.Application.Invocation 
{
    public class FormControllerInvocation : IFormControllerInvocation
    {
        protected readonly IFormActionResolver _formActionResolver;
        protected readonly IApplicationSetupService _applicationSetupService;
        protected readonly ISecurityService _securityService;
        protected readonly ILogger<FormControllerInvocation> _logger;
        protected readonly ISessionAccessor _session;

        public FormControllerInvocation(
            IFormActionResolver formActionResolver,
            ISecurityService securityService,
            IApplicationSetupService applicationSetupService,
            ILogger<FormControllerInvocation> logger, 
            ISessionAccessor session)
        {
            this._formActionResolver = formActionResolver;
            this._securityService = securityService;
            this._applicationSetupService = applicationSetupService;
            this._logger = logger;
            this._session = session;
        }

        public virtual IFormWrapper Invoke(IFormWrapper data, IFormController controller)
        {
            IFormWrapper response = new FormWrapper();

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

                response = this._formActionResolver.InvokeAction(data, controller);

                response.SetSessionData(this._session.GetInnerDictionary());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Form - FormControllerInvocation - Invoke");

                response.SetError(ex.Message);
            }

            return response;
        }
    }
}
