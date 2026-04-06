using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CalleDesdeCaracterUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _calleHasta;


        public CalleDesdeCaracterUbicacionValidationRule(string valor, string calleHasta)
        {
            this._value = valor;
            this._calleHasta = calleHasta;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            int calleHasta = Encoding.ASCII.GetBytes(_calleHasta.ToUpper())[0];
            int value = Encoding.ASCII.GetBytes(_value.ToUpper())[0];

            if (calleHasta < value)
            {
                errors.Add(new ValidationError("General_Sec0_Error_CaracterCalleDesdeMenorHasta", new List<string>() { this._value.ToString(), this._calleHasta.ToString()}));
            }

            return errors;
        }

    }
}
