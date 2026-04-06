using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class BloqueHastaCaracterUbicacionValidationRule : IValidationRule
    {
        protected readonly string _bloqueDesde;
        protected readonly string _value;


        public BloqueHastaCaracterUbicacionValidationRule(string bloqueDesde, string valor)
        {
            this._bloqueDesde = bloqueDesde;
            this._value = valor;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            int bloqueDesde = Encoding.ASCII.GetBytes(_bloqueDesde.ToUpper())[0];
            int value = Encoding.ASCII.GetBytes(_value.ToUpper())[0];

            if (bloqueDesde > value)
            {
                errors.Add(new ValidationError("General_Sec0_Error_CaracterBloqueHastaMenorDesde", new List<string>() { this._value.ToString() , _bloqueDesde }));
            }

            return errors;
        }

    }
}
