using System;
using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DecimalLowerThanValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IFormatProvider _proveedor;
        protected readonly decimal _maxValue;

        public DecimalLowerThanValidationRule(IFormatProvider proveedorDeFormato, string value, decimal maxValue)
        {
            this._value = value;
            this._maxValue = maxValue;
            this._proveedor = proveedorDeFormato;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            decimal value = decimal.Parse(this._value, this._proveedor);

            if (value > this._maxValue)
                errors.Add(new ValidationError("General_Sec0_Error_NumberCantBeGreaterThan", new List<string> { this._maxValue.ToString() }));

            return errors;
        }
    }
}
