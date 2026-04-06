using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class StringDescripcionSoloUpperValidationRule : IValidationRule
    {
        protected readonly string _valueString;

        public StringDescripcionSoloUpperValidationRule(string valueString)
        {
            this._valueString = valueString;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();


            if (!this._valueString.Equals(this._valueString.ToUpper()))
                errors.Add(new ValidationError("General_Sec1_Error_TextoSoloEnMayuscula"));

            return errors;
        }
    }
    
    
}
