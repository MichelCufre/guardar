using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PositiveShortLessThanValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly short _maxValue;

        public PositiveShortLessThanValidationRule(string value, short maxValue)
        {
            this._value = value;
            this._maxValue = maxValue;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            short value;

            if (short.TryParse(this._value, out value))
                if(value > this._maxValue)
                    errors.Add(new ValidationError("General_Sec0_Error_Error88", new List<string>() { this._value.ToString(), this._maxValue.ToString() }));

            return errors;
        }
    }
}
