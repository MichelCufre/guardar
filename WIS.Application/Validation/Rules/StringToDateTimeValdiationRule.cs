using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class StringToDateTimeValdiationRule : IValidationRule
    {
        protected readonly string _valueDateString;

        public StringToDateTimeValdiationRule(string valueDateString)
        {
            this._valueDateString = valueDateString;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var currCulture = CultureInfo.CurrentCulture;

            var formatString = CDateFormats.DATETIME_24H; //"dd/MM/yyyy HH:mm:ss";

            if (this._valueDateString.Length == 10)
                formatString = CDateFormats.DATE_ONLY; // "dd/MM/yyyy";

            if (this._valueDateString.Length == 16)
                formatString = CDateFormats.DATETIME_HHmm;// "dd/MM/yyyy HH:mm";

            if (this._valueDateString.Length == 24)
                formatString = CDateFormats.DATETIME_24HTT; //"dd/MM/yyyy HH:mm:ss tt";

            if (this._valueDateString.Length == 18)
                formatString = CDateFormats.DATETIME_HMMSS;// ""dd/MM/yyyy h:mm:ss";

            try
            {
                DateTime.ParseExact(this._valueDateString, formatString, currCulture);
            }
            catch
            {
                errors.Add(new ValidationError("General_Sec0_Error_Error36"));
            }

            return errors;
        }
    }
}
