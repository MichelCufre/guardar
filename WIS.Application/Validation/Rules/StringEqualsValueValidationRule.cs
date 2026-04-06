using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class StringEqualsValueValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _comparingValue;
        protected readonly string _errorMessage;

        public StringEqualsValueValidationRule(string value, string comparingValue, string errorMessage)
        {
            this._value = value;
            this._comparingValue = comparingValue;
            this._errorMessage = errorMessage;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._value.Equals(this._comparingValue))
            {
                if (string.IsNullOrEmpty(this._errorMessage))
                    errors.Add(new ValidationError("General_Sec0_Error_Error40"));
                else
                    errors.Add(new ValidationError(this._errorMessage));
            }

            return errors;
        }
    }
}
