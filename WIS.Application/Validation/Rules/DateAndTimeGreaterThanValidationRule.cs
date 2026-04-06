using System;
using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DateAndTimeGreaterThanValidationRule : IValidationRule
    {
        protected readonly DateTime _date1;
        protected readonly DateTime _date2;
        protected readonly string _messageDate1GreaterDate2;

        public DateAndTimeGreaterThanValidationRule(DateTime date1, DateTime date2, string messageDate1GreaterDate2 = null)
        {
            this._date1 = date1;
            this._date2 = date2;
            this._messageDate1GreaterDate2 = messageDate1GreaterDate2;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._date1 > this._date2)
                errors.Add(new ValidationError(this._messageDate1GreaterDate2 ?? "General_Sec0_Error_InvalidDate"));

            return errors;
        }
    }
}
