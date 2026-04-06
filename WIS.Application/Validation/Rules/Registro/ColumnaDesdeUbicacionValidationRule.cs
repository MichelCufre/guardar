using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ColumnaDesdeUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _columnaHasta;


        public ColumnaDesdeUbicacionValidationRule(string valor, string columnaHasta)
        {
            this._value = valor;
            this._columnaHasta = columnaHasta;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_columnaHasta, out short columnaHasta) && short.TryParse(_value, out short valor))
            {
                if (columnaHasta < valor)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_NumeroColumnaDesdeMenorHasta", new List<string>() { this._columnaHasta.ToString(), this._value.ToString() }));
                }
            }

            return errors;
        }

    }
}
