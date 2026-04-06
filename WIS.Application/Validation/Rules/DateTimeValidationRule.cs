using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DateTimeValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;

        public DateTimeValidationRule(string valueDateString)
        {
            this._valueDateString = valueDateString;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!DateTimeExtension.TryParseFromIso(this._valueDateString, out DateTime? parsedDate))
                errors.Add(new ValidationError("General_Sec0_Error_InvalidDate"));

            return errors;
        }
    }
}
