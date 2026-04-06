using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CalleDesdeUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _calleHasta;


        public CalleDesdeUbicacionValidationRule(string valor, string calleHasta)
        {
            this._value = valor;
            this._calleHasta = calleHasta;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_calleHasta, out short calleHasta) && short.TryParse(_value, out short valor))
            {
                if (calleHasta < valor)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_NumeroCalleDesdeMenorHasta", new List<string>() { this._calleHasta.ToString(), this._value.ToString() }));
                }
            }

            return errors;
        }

    }
}
