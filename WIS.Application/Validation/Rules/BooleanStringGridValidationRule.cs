using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class BooleanStringGridValidationRule : IValidationRule
    {
        protected readonly string _Value;

        public BooleanStringGridValidationRule(string Value)
        {
            this._Value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (_Value != "S" && _Value != "N")
                errors.Add(new ValidationError("General_Sec0_Error_BooleanStringGrid"));

            return errors;
        }
    }
}