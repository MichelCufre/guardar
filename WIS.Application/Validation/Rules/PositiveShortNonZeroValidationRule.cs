using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PositiveShortNonZeroValidationRule : IValidationRule
    {
        protected readonly string _value;

        public PositiveShortNonZeroValidationRule(string value)
        {
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._value))
                return errors;

            if (!short.TryParse(this._value, out short parsedValue) || parsedValue <= 0)
                errors.Add(new ValidationError("General_Sec0_Error_Error27"));

            return errors;
        }
    }
}
