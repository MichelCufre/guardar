using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class ProductoAceptaDecimalesValidationRule : IValidationRule
    {
        protected readonly bool _aceptaDecimal;
        protected readonly string _cantidad;
        protected readonly IFormatProvider _culture;

        public ProductoAceptaDecimalesValidationRule(bool aceptaDecimal, string cantidad, IFormatProvider culture)
        {
            this._aceptaDecimal = aceptaDecimal;
            this._cantidad = cantidad;
            this._culture = culture;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (decimal.TryParse(this._cantidad, NumberStyles.Number, _culture, out decimal value))
            {
                if (!this._aceptaDecimal && value != Math.Truncate(value))
                    errors.Add(new ValidationError("General_Sec0_Error_ProductoNoAceptaDecimales"));
            }
            else
                errors.Add(new ValidationError("General_Sec0_Error_Error14"));

            return errors;
        }
    }
}
