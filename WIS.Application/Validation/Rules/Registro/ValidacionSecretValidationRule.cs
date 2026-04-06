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
    public class ValidacionSecretValidationRule : IValidationRule
    {
        protected readonly string _secret;

        public ValidacionSecretValidationRule(string secret)
        {
            this._secret = secret;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._secret))
            {
                //TODO:
            }

            return errors;
        }
    }
}