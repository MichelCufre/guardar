using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Security;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Repositories;
using WIS.Domain.Security;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class ValidarPasswordValidationRule : IValidationRule
    {
        protected readonly string _loginName;
        protected readonly string _password;
        protected readonly IAuthorizationService _authService;

        public ValidarPasswordValidationRule(IAuthorizationService authService, string loginName, string password)
        {
            _loginName = loginName;
            _password = password;
            _authService = authService;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var model = new ValidateCurrentPassword()
            {
                LoginName = _loginName,
                Password = _password
            };

            try
            {
                _authService.ValidateCurrentPassword(model);
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError("General_ORT090_Error_ContraseñaIncorrecta"));
            }

            return errors;
        }
    }
}
