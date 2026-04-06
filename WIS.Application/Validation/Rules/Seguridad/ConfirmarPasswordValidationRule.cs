using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ConfirmarPasswordValidationRule : IValidationRule
    {
        protected readonly string _password;
        protected readonly string _repassword;

        public ConfirmarPasswordValidationRule(string repassword, string password)
        {
            this._password = password;
            this._repassword = repassword;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._password) && !string.IsNullOrEmpty(this._repassword))
            {
                if (!this._password.Equals(this._repassword))
                {
                    errors.Add(new ValidationError("General_Sec0_Error_contraseniasNoCoinciden"));
                }
            }

            return errors;
        }
    }
}
