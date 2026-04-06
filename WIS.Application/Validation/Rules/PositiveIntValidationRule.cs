using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PositiveIntValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly bool _allowZero;

        public PositiveIntValidationRule(string value, bool allowZero = true)
        {
            this._value = value;
            this._allowZero = allowZero;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._value))
                return errors;

            if (!int.TryParse(this._value, out int parsedValue))
                errors.Add(new ValidationError("General_Sec0_Error_Error67"));
            else if (this._allowZero && parsedValue < 0)
                errors.Add(new ValidationError("General_Sec0_Error_Error27"));
            else if (!this._allowZero && parsedValue <= 0)
                errors.Add(new ValidationError("General_Sec0_Error_MayorACero"));

            return errors;
        }
    }
}
