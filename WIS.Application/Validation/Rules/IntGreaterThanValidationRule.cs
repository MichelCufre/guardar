using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class IntGreaterThanValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _minValue;

        public IntGreaterThanValidationRule(string value, int minValue)
        {
            this._value = value;
            this._minValue = minValue;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            int value = int.Parse(this._value);

            if (value <= this._minValue)
                errors.Add(new ValidationError("General_Sec0_Error_Error78", new List<string> { this._minValue.ToString() }));

            return errors;
        }
    }
}
