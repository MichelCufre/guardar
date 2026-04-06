
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class BloqueDesdeUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _bloqueDesde;


        public BloqueDesdeUbicacionValidationRule(string valor, string bloqueDesde)
        {
            this._value = valor;
            this._bloqueDesde = bloqueDesde;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_bloqueDesde, out short bloqueDesde) && short.TryParse(_value, out short valor))
            {
                if (bloqueDesde > valor)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_NumeroBloqueDesdeMenorHasta", new List<string>() { this._bloqueDesde.ToString(), this._value.ToString() }));
                }
            }

            return errors;
        }

    }
}
