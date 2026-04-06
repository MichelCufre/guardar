using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PositiveDecimalValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly bool _allowZero;
        protected readonly IFormatProvider _proveedor;

        public PositiveDecimalValidationRule(IFormatProvider proveedorDeFormato, string value, bool allowZero = true)
        {
            this._value = value;
            this._proveedor = proveedorDeFormato;
            this._allowZero = allowZero;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._value))
                return errors;
            var separador = ((CultureInfo)_proveedor).NumberFormat.NumberDecimalSeparator;

            decimal parsedValue = -1;

            string msgFormato = "General_Sec0_Error_Error14";
            if (separador == ",")
            {
                if (this._value.Contains("."))
                    errors.Add(new ValidationError("General_Sec0_Error_FormatoIncorrecto", new List<string> { separador }));
            }
            else if (this._value.Contains(","))
                errors.Add(new ValidationError("General_Sec0_Error_FormatoIncorrecto", new List<string> { separador }));

            if (errors.Any())
                return errors;

            if (!decimal.TryParse(this._value, NumberStyles.Any, _proveedor, out parsedValue))
                errors.Add(new ValidationError(msgFormato));
            else if (parsedValue < 0 || (!this._allowZero && parsedValue == 0))
                errors.Add(new ValidationError("General_Sec0_Error_Error26"));

            return errors;
        }
    }
}
