using System;
using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class UrlValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly UriKind _uriKind;

        public UrlValidationRule(string value, UriKind uriKind = UriKind.Absolute)
        {
            this._value = value;
            this._uriKind = uriKind;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!Uri.IsWellFormedUriString(_value, _uriKind))
                errors.Add(new ValidationError("General_Sec0_Error_Error14"));
            return errors;
        }
    }
}
