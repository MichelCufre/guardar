using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PositiveOrNegativeDecimalValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IFormatProvider _proveedor;

        public PositiveOrNegativeDecimalValidationRule(IFormatProvider proveedorDeFormato, string value)
        {
            this._value = value;
            this._proveedor = proveedorDeFormato;
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
                    errors.Add(new ValidationError(msgFormato));
            }
            else if (this._value.Contains(","))
                errors.Add(new ValidationError(msgFormato));

            if (errors.Any())
                return errors;

            if (!decimal.TryParse(this._value, out parsedValue))
                errors.Add(new ValidationError(msgFormato));

            return errors;
        }
    }
}
