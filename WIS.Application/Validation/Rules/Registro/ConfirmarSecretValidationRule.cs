using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ConfirmarSecretValidationRule : IValidationRule
    {
        protected readonly string _secret;
        protected readonly string _resecret;

        public ConfirmarSecretValidationRule(string resecret, string secret)
        {
            this._secret = secret;
            this._resecret = resecret;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._secret) && !string.IsNullOrEmpty(this._resecret))
            {
                if (!this._secret.Equals(this._resecret))
                {
                    errors.Add(new ValidationError("General_Sec0_Error_secretosNoCoinciden"));
                }
            }

            return errors;
        }
    }
}