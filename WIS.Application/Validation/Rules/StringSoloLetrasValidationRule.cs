using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class StringSoloLetrasValidationRule : IValidationRule
    {
        protected readonly string _valueString;

        public StringSoloLetrasValidationRule(string valueString)
        {
            this._valueString = valueString;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!Regex.IsMatch(this._valueString, @"^[a-zA-Z]+$"))
            {
                errors.Add(new ValidationError("General_Sec0_Error_TextoConCaracteresNumericos"));
            }

            return errors;
        }
    }
}
