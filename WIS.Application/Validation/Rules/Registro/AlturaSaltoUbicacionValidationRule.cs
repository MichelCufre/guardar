using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class AlturaSaltoUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _alturaDesde;
        protected readonly string _alturaHasta;


        public AlturaSaltoUbicacionValidationRule(string valor, string alturaDesde, string alturaHasta)
        {
            this._value = valor;
            this._alturaDesde = alturaDesde;
            this._alturaHasta = alturaHasta;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(this._value, out short valor) && short.TryParse(this._alturaDesde, out short alturaDesde) && short.TryParse(this._alturaHasta, out short alturaHasta))
            {
                //if (alturaHasta > alturaDesde && valor > (alturaHasta - alturaDesde))
                //{
                //    errors.Add(new ValidationError("General_Sec0_Error_SaltoMenorADesde", new List<string>() { alturaHasta.ToString() }));
                //}
                //else
                if (alturaHasta == alturaDesde && valor <= 0)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_SaltoMayorAUno", new List<string>() { "0" }));
                }
            }

            return errors;
        }

    }
}
