using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class IntLowerThanValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _maxValue;

        public IntLowerThanValidationRule(string value, int maxValue)
        {
            this._value = value;
            this._maxValue = maxValue;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            int value = int.Parse(this._value);

            if (value > this._maxValue)
                errors.Add(new ValidationError("General_Sec0_Error_NumberCantBeGreaterThan", new List<string> { this._maxValue.ToString() }));

            return errors;
        }
    }
}
