using System;
using System.Collections.Generic;
using System.Text;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DateTimeGreaterThanCurrentDateAndTimeValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;
        protected readonly string _message;
        protected readonly bool _validarHora;

        public DateTimeGreaterThanCurrentDateAndTimeValidationRule(string valueDateString, string message = null, bool validarHora = true)
        {
            this._valueDateString = valueDateString;
            this._validarHora = validarHora;
            this._message = message;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            DateTime? dateIso = DateTimeExtension.ParseFromIso(this._valueDateString);

            if (dateIso != null)
            {
                DateTime date = (DateTime)dateIso;

                if (date.Date < DateTime.Today)
                {
                    errors.Add(new ValidationError(this._message ?? "General_Sec0_Error_DateLowerThanCurrentDate"));
                }
                else if (date.Date == DateTime.Today && _validarHora)
                {
                    if (date.Hour < DateTime.Now.Hour)
                    {
                        errors.Add(new ValidationError(this._message ?? "General_Sec0_Error_DateLowerThanCurrentDate"));
                    }
                    else if (date.Hour == DateTime.Now.Hour && date.Minute < DateTime.Now.Minute)
                    {
                        errors.Add(new ValidationError(this._message ?? "General_Sec0_Error_DateLowerThanCurrentDate"));
                    }
                }
            }

            return errors;
        }
    }
}
