using WIS.Domain.DataModel;
using WIS.Security;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Domain.Security;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation;
using WIS.Exceptions;
using System;
using System.Linq;
using WIS.Domain.Services.Interfaces;

namespace WIS.Application.Controllers.SEG
{
    public class SEG070CambiarPassword : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IAuthorizationService _authService;

        public SEG070CambiarPassword(IUnitOfWorkFactory uowFactory, IIdentityService identity, IFormValidationService formValidationService, IAuthorizationService authService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._authService = authService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            try
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                var usuario = uow.SecurityRepository.GetUsuario(_identity.UserId);
                if (usuario == null)
                    throw new ValidationFailedException("SEC030_Sec0_Error_NoSePudoObtenerUsuario");

                var model = new ChangePassword()
                {
                    LoginName = usuario.Username,
                    Password = form.GetField("passwordOld").Value,
                    NewPassword = form.GetField("passwordNueva").Value,
                    ConfirmPassword = form.GetField("rePasswordNueva").Value
                };

                _authService.ChangePassword(model);

                context.AddSuccessNotification("SEG070_msg_Sucess_Reloguearse");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }

            return form;
        }
        
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new CambioPasswordValidationModule(uow, this._identity.UserId, this._identity.Application), form, context);
        }

    }
}
