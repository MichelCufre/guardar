using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PositiveNumberMaxLengthValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _maxLength;

        public PositiveNumberMaxLengthValidationRule(string value,int maxLength)
        {
            this._value = value;
            this._maxLength = maxLength;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            bool aux = false;
            string pattern = @"^\d{1,10}$"; // solo enteros
            int cantErrores = 0;

            if (string.IsNullOrEmpty(this._value))
                return errors;

            if (this._value.Length > this._maxLength)
            {
                errors.Add(new ValidationError("General_Sec0_Error_Error64", new List<string> { this._maxLength.ToString() }));
                cantErrores += 1;
            }
            

            aux = Regex.IsMatch(this._value, pattern);
            if (!aux && cantErrores == 0)
            {
                errors.Add(new ValidationError("General_Sec0_Error_Error14"));
                cantErrores += 1;
            }
            

            if (aux)
            {
                aux = int.TryParse(this._value, out int outValue);
                if (!aux)
                    errors.Add(new ValidationError("General_Sec0_Error_Error15"));
            }

            return errors;
        }
    }
}
