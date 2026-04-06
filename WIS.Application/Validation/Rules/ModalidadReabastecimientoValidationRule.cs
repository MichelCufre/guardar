using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ModalidadReabastecimientoValidationRule : IValidationRule
    {
        protected readonly string _modalidad;
        public ModalidadReabastecimientoValidationRule( string modalidad)
        {
            this._modalidad = modalidad;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!EstrategiaAlmacenajeReabastecimientoDb.GetModalidadesReabastecimiento().Any(t => t == _modalidad))
                errors.Add(new ValidationError("General_Sec0_Error_Error99"));

            return errors;
        }
    }
}
