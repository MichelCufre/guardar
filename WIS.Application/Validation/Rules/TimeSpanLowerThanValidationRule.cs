using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class TimeSpanLowerThanValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;
        protected readonly TimeSpan _maxTimeSpan;
        protected readonly IFormatProvider _formatProvider;

        public TimeSpanLowerThanValidationRule(string valueDateString, TimeSpan maxTimeSpan, IFormatProvider formatProvider)
        {
            this._valueDateString = valueDateString;
            this._maxTimeSpan = maxTimeSpan;
            this._formatProvider = formatProvider;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var timeSpanToCompare = TimeSpan.Parse(this._valueDateString, this._formatProvider);

            if(timeSpanToCompare > this._maxTimeSpan)
                errors.Add(new ValidationError("General_Sec0_Error_InvalidTimeSpan", new List<string> { _maxTimeSpan.ToString("HH:mm", this._formatProvider) }));

            return errors;
        }
    }
}
