using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DateTimeLowerThanValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;
        protected readonly string _valueDateReference;
        protected readonly string _message;

        public DateTimeLowerThanValidationRule(string valueDateString, string valueDateReference, string message)
        {
            this._valueDateString = valueDateString;
            this._valueDateReference = valueDateReference;
            this._message = message;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (DateTimeExtension.TryParseFromIso(this._valueDateReference, out DateTime? dateComparison))
            {
                DateTime? date = DateTimeExtension.ParseFromIso(this._valueDateString);

                if (date > dateComparison)
                    errors.Add(new ValidationError(this._message ?? "General_Sec0_Error_InvalidDate"));
            }

            return errors;
        }
    }
}
