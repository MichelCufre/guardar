using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class NumberValidationRule<T> : IValidationRule
    {
        protected readonly string _value;
        protected readonly IFormatProvider _culture;


        public NumberValidationRule(string value, IFormatProvider culture)
        {
            this._value = value;
            this._culture = culture;

        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            try
            {
                _value.ToNumber<T>();
            }
            catch (Exception)
            {
                errors.Add(new ValidationError("General_Sec0_Error_Error26"));
            }

            if (!decimal.TryParse(this._value, NumberStyles.Number, _culture, out decimal parsedValue) || parsedValue < 0)
                errors.Add(new ValidationError("General_Sec0_Error_Error26"));

            return errors;
        }
    }
}
