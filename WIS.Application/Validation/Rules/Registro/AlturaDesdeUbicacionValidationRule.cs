using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class AlturaDesdeUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _alturaDesde;


        public AlturaDesdeUbicacionValidationRule(string valor, string alturaDesde)
        {
            this._value = valor;
            this._alturaDesde = alturaDesde;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_alturaDesde, out short alturaDesde) && short.TryParse(_value, out short valor))
            {
                if (alturaDesde > valor)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_NumeroAlturaDesdeMenorHasta", new List<string>() { this._value.ToString(), this._alturaDesde.ToString() }));
                }
            }

            return errors;
        }

    }
}
