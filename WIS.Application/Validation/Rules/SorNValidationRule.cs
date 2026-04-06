
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class SorNValidationRule : IValidationRule
    {
        protected readonly string _valueString;

        public SorNValidationRule(string valueString)
        {
            this._valueString = valueString.ToUpper();
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(_valueString))
            {
                if (_valueString != "S" && _valueString != "N" && _valueString != " ")
                {
                    errors.Add(new ValidationError("General_Sec0_Error_StringNoBoolean"));
                }
            }
            

            return errors;
        }
    }
}
