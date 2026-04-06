using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class StringSoloUpperValidationRule : IValidationRule
    {
        protected readonly string _valueString;

        public StringSoloUpperValidationRule(string valueString)
        {
            this._valueString = valueString;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();


            if (!this._valueString.Equals(this._valueString.ToUpper()))
                errors.Add(new ValidationError("General_Sec0_Error_TextoSoloEnMayuscula"));

            return errors;
        }
    }
}
