using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class StringIsoDateToDateTimeValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;

        public StringIsoDateToDateTimeValidationRule(string valueDateString)
        {
            this._valueDateString = valueDateString;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            DateTime? parsedDate;
            if (!this._valueDateString.TryParseFromIso(out parsedDate))
            {
                errors.Add(new ValidationError("General_Sec0_Error_Error35"));
            }

            return errors;
        }
    }
}
