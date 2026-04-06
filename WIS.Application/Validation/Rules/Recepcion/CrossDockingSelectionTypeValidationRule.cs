using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Recepcion;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class CrossDockingSelectionTypeValidationRule : IValidationRule
    {
        protected readonly string _value;

        public CrossDockingSelectionTypeValidationRule(string value)
        {
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var tipo = (CrossDockingSeleccionTipo)int.Parse(this._value);

            if (tipo != CrossDockingSeleccionTipo.Todo && tipo != CrossDockingSeleccionTipo.OrdenDeCompra)
                errors.Add(new ValidationError("General_Sec0_Error_InvalidCrossDockingSelectionType"));

            return errors;
        }
    }
}
