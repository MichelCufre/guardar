using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CalleHastaUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _calleDesde;


        public CalleHastaUbicacionValidationRule(string valor, string calleDesde)
        {
            this._value = valor;
            this._calleDesde = calleDesde;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_calleDesde, out short calleDesde) && short.TryParse(_value, out short valor))
            {
                if (calleDesde > valor)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_NumeroCalleHastaMenorADesde", new List<string>() { this._value.ToString(), this._calleDesde.ToString()}));
                }
            }

            return errors;
        }

    }
}
