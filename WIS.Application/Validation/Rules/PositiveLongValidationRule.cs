using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PositiveLongValidationRule : IValidationRule
    {
        protected readonly string _value;

        public PositiveLongValidationRule(string value)
        {
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._value))
                return errors;
            
            if (!long.TryParse(this._value, out long parsedValue) || parsedValue < 0)
                errors.Add(new ValidationError("General_Sec0_Error_Error27"));

            return errors;
        }
    }
}
