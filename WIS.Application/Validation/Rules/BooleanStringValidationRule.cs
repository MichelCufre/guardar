using System.Collections.Generic;
using System.Linq;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class BooleanStringValidationRule : IValidationRule
    {
        protected readonly string _Value;

        public BooleanStringValidationRule(string Value)
        {
            this._Value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if(_Value != "true" && _Value != "false")
                errors.Add(new ValidationError("General_Sec0_Error_BooleanString"));

            return errors;
        }
    }
}
