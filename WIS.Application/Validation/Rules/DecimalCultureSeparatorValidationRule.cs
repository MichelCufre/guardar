using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DecimalCultureSeparatorValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IFormatProvider _proveedor;

        public DecimalCultureSeparatorValidationRule(IFormatProvider proveedorDeFormato, string value)
        {
            this._value = value;
            this._proveedor = proveedorDeFormato;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var separador = ((CultureInfo)_proveedor).NumberFormat.NumberDecimalSeparator;

            if (separador == ",")
            {
                if (this._value.Contains("."))
                    errors.Add(new ValidationError("General_Sec0_Error_SepadoresEsComa"));
            }
            else if (this._value.Contains(","))
                errors.Add(new ValidationError("General_Sec0_Error_SepadoresEsPunto"));

            return errors;
        }
    }
}
