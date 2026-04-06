using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class StringMinLengthValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _maxLength;


        public StringMinLengthValidationRule(string value,int maxLength)
        {
            this._value = value;
            this._maxLength = maxLength;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (!string.IsNullOrEmpty(this._value))
            {
                if (this._value.Length < this._maxLength)
                    errors.Add(new ValidationError("General_Sec0_Error_Error92", new List<string> { this._maxLength.ToString() }));

            }

            return errors;
        }
    }
}
