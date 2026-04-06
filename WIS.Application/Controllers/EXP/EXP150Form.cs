using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EXP
{
    public class EXP150Form : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public EXP150Form(
            ISecurityService security,
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService)
        {
            this._security = security;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            return form;
        }
    }
}
