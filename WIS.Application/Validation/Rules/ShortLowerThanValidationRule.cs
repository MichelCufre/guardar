using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ShortLowerThanValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly short _maxValue;

        public ShortLowerThanValidationRule(string value, short maxValue)
        {
            this._value = value;
            this._maxValue = maxValue;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            short value = short.Parse(this._value);

            if (value > this._maxValue)
                errors.Add(new ValidationError("General_Sec0_Error_NumberCantBeGreaterThan", new List<string> { this._maxValue.ToString() }));

            return errors;
        }
    }
}
