using System.Collections.Generic;
using System.Linq;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class IsTrueValidationRule : IValidationRule
    {
        protected readonly string _Value;

        public IsTrueValidationRule(string Value)
        {
            this._Value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if(_Value != "true" || _Value != "S")
                errors.Add(new ValidationError("General_Sec0_Error_IsTrueValue"));

            return errors;
        }
    }
}
