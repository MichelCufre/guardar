using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class NumeroEnteroValidationRule : IValidationRule
    {
        protected readonly string _value;

        public NumeroEnteroValidationRule(string value)
        {
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._value))
                return errors;

            if (!int.TryParse(this._value, out int parsedValue))
                errors.Add(new ValidationError("General_Sec0_Error_Error67"));

            return errors;
        }
    }
}
