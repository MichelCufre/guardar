using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class TimeSpanGreaterThanValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;
        protected readonly TimeSpan _minTimeSpan;
        protected readonly IFormatProvider _formatProvider;

        public TimeSpanGreaterThanValidationRule(string valueDateString, TimeSpan maxTimeSpan, IFormatProvider formatProvider)
        {
            this._valueDateString = valueDateString;
            this._minTimeSpan = maxTimeSpan;
            this._formatProvider = formatProvider;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var timeSpanToCompare = TimeSpan.Parse(this._valueDateString, this._formatProvider);

            if (timeSpanToCompare < this._minTimeSpan)
                errors.Add(new ValidationError("General_Sec0_Error_InvalidTimeSpan", new List<string> { _minTimeSpan.ToString("c", this._formatProvider) }));

            return errors;
        }
    }
}
