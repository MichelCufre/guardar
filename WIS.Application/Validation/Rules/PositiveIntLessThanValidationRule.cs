using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PositiveIntLessThanValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _maxValue;

        public PositiveIntLessThanValidationRule(string value, int maxValue)
        {
            this._value = value;
            this._maxValue = maxValue;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!int.TryParse(this._value, out int intValue) || intValue < 0)
            {
                errors.Add(new ValidationError("General_Sec0_Error_Error27", new List<string> { _value }));
                return errors;
            }

            if (intValue > this._maxValue)
                errors.Add(new ValidationError("General_Sec0_Error_Error84", new List<string>() { _maxValue.ToString() }));

            return errors;
        }
    }
}
