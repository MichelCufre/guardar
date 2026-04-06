using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class KeepNullWhenOldWasValidationRule : IValidationRule
    {
        protected string _value;
        protected string _oldValue;

        public KeepNullWhenOldWasValidationRule(string currentValue, string oldValue)
        {
            _value = currentValue;
            _oldValue = oldValue;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(_oldValue) && !string.IsNullOrEmpty(_value))
                errors.Add(new ValidationError("AUT100Caracteristicas_Sec0_Error_CannotUpdateNullValue"));

            return errors;
        }
    }
}
