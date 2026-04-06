using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ValorArbitrajeValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _valueMoneda;
        protected readonly IFormatProvider _culture;


        public ValorArbitrajeValidationRule(string value, string valueMoneda, IFormatProvider culture)
        {
            this._value = value;
            this._valueMoneda = valueMoneda;
            this._culture = culture;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            decimal value = decimal.Parse(this._value, this._culture);

            if (!string.IsNullOrEmpty(this._valueMoneda) && this._valueMoneda != "1" && string.IsNullOrEmpty(this._value))
                errors.Add(new ValidationError("General_Sec0_Error_Error25"));

            return errors;
        }
    }
}
