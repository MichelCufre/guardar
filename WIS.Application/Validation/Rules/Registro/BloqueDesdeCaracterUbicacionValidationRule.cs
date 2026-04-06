using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class BloqueDesdeCaracterUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _bloqueDesde;


        public BloqueDesdeCaracterUbicacionValidationRule(string valor, string bloqueDesde)
        {
            this._value = valor;
            this._bloqueDesde = bloqueDesde;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            int bloqueDesde = Encoding.ASCII.GetBytes(_bloqueDesde.ToUpper())[0];
            int value = Encoding.ASCII.GetBytes(_value.ToUpper())[0];

            if (bloqueDesde > value)
            {
                errors.Add(new ValidationError("General_Sec0_Error_CaracterBloqueDesdeMenorHasta", new List<string>() { this._bloqueDesde.ToString(), this._value.ToString()}));
            }

            return errors;
        }

    }
}
