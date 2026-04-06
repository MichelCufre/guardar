using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class TimeSpanValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;
        protected readonly IFormatProvider _formatProvider;

        public TimeSpanValidationRule(string valueDateString, IFormatProvider formatProvider)
        {
            this._valueDateString = valueDateString;
            this._formatProvider = formatProvider;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!TimeSpan.TryParse(this._valueDateString, this._formatProvider, out _))
                errors.Add(new ValidationError("General_Sec0_Error_InvalidTimeSpan"));

            return errors;
        }
    }
}
