using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CalleHastaCaracterUbicacionValidationRule : IValidationRule
    {
        protected readonly string _calleDesde;
        protected readonly string _value;


        public CalleHastaCaracterUbicacionValidationRule(string calleDesde, string valor)
        {
            this._calleDesde = calleDesde;
            this._value = valor;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            int calleDesde = Encoding.ASCII.GetBytes(_calleDesde.ToUpper())[0];
            int value = Encoding.ASCII.GetBytes(_value.ToUpper())[0];

            if (calleDesde > value)
            {
                errors.Add(new ValidationError("General_Sec0_Error_CaracterCalleHastaMenorDesde", new List<string>() { this._value.ToString(), _calleDesde }));
            }

            return errors;
        }

    }
}
