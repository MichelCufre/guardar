using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ColumnaHastaUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _columnaDesde;


        public ColumnaHastaUbicacionValidationRule(string valor, string columnaDesde)
        {
            this._value = valor;
            this._columnaDesde = columnaDesde;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();


            if (short.TryParse(_columnaDesde, out short columnaDesde) && short.TryParse(_value, out short valor))
            {
                if (columnaDesde > valor)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_NumeroColumnaHastaMenorADesde", new List<string>() { this._columnaDesde.ToString(), this._value.ToString() }));
                }
            }

            return errors;
        }

    }
}
