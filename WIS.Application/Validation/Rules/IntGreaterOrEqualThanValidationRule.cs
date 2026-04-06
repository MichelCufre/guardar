using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class IntGreaterOrEqualThanValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _comparador;

        public IntGreaterOrEqualThanValidationRule(string value, int comparador)
        {
            this._value = value;
            this._comparador = comparador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            int valorEntero = int.Parse(this._value);

            if (valorEntero < this._comparador)
                errors.Add(new ValidationError("General_Sec0_Error_NumeroDebeSerMayorIgualA", new List<string>() { this._comparador.ToString() }));

            return errors;
        }
    }
}
