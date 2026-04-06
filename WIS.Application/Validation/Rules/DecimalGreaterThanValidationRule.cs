using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DecimalGreaterThanValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IFormatProvider _proveedor;
        protected readonly decimal _minValue;

        public DecimalGreaterThanValidationRule(IFormatProvider proveedorDeFormato, string value, decimal minValue)
        {
            this._value = value;
            this._minValue = minValue;
            this._proveedor = proveedorDeFormato;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            decimal value = decimal.Parse(this._value, this._proveedor);

            if (value < this._minValue)
                errors.Add(new ValidationError("General_Sec0_Error_Error13", new List<string> { this._minValue.ToString() }));

            return errors;
        }
    }
}
