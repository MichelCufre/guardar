using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ColumnaSaltoUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _columnaDesde;
        protected readonly string _columnaHasta;


        public ColumnaSaltoUbicacionValidationRule(string valor, string columnaDesde, string columnaHasta)
        {
            this._value = valor;
            this._columnaDesde = columnaDesde;
            this._columnaHasta = columnaHasta;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_value, out short valor) && short.TryParse(_columnaDesde, out short columnaDesde) && short.TryParse(_columnaHasta, out short columnaHasta))
            {

                //if (columnaHasta > columnaDesde && valor > (columnaHasta - columnaDesde))
                //{
                //    errors.Add(new ValidationError("General_Sec0_Error_SaltoMenorADesde", new List<string>() { columnaHasta.ToString() }));
                //}
                //else
                if (columnaHasta == columnaDesde && valor <= 0)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_SaltoMayorAUno", new List<string>() { "0" }));
                }
            }

            return errors;
        }

    }
}
