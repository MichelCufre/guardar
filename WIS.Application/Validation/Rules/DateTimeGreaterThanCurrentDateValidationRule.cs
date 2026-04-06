using System;
using System.Collections.Generic;
using System.Text;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DateTimeGreaterThanCurrentDateValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;
        protected readonly string _message;

        public DateTimeGreaterThanCurrentDateValidationRule(string valueDateString, string message = null)
        {
            this._valueDateString = valueDateString;
            this._message = message;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            DateTime? date = DateTimeExtension.ParseFromIso(this._valueDateString);

            if (date < DateTime.Today)
                errors.Add(new ValidationError(this._message ?? "General_Sec0_Error_DateLowerThanCurrentDate"));

            return errors;
        }
    }
}
