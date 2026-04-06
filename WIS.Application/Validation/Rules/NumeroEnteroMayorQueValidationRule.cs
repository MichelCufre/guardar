using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    [Obsolete("Utilizar IntGreaterThanValidationRule")]
    public class NumeroEnteroMayorQueValidationRule : IValidationRule
    {
        protected readonly int _value;
        protected readonly int _comparador;

        public NumeroEnteroMayorQueValidationRule(int value, int comparador)
        {
            this._value = value;
            this._comparador = comparador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._value <= this._comparador)
                errors.Add(new ValidationError("General_Sec0_Error_Error78", new List<string>() { this._comparador.ToString() }));

            return errors;
        }
    }
}
