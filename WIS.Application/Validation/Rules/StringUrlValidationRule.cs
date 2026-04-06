using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class StringUrlValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _nombreCampo;

        public StringUrlValidationRule(string value)
        {
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._value))
            {
                Regex pattern = new Regex(@"^(http|ftp|https|www)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$", RegexOptions.IgnoreCase);
                Match match = pattern.Match(_value);
                if (!match.Success)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_ErrorURL"));
                }
            }

            return errors;
        }
    }
}
