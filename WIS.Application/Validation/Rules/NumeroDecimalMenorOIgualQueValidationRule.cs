using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class NumeroDecimalMenorOIgualQueValidationRule : IValidationRule
    {
        protected readonly decimal _value;
        protected readonly decimal _comparador;

        public NumeroDecimalMenorOIgualQueValidationRule(decimal value, decimal comparador)
        {
            this._value = value;
            this._comparador = comparador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._value > this._comparador)
                errors.Add(new ValidationError("General_Sec0_Error_Error84", new List<string>() { this._comparador.ToString() }));

            return errors;
        }
    }
}
