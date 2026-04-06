using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class AgrupadorPedidoValidationRule : IValidationRule
    {
        protected readonly string _agrupador;

        public AgrupadorPedidoValidationRule(string agrupador)
        {
            this._agrupador = agrupador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            //List<TipoAgrupacion>

            return errors;
        }
    }
}
