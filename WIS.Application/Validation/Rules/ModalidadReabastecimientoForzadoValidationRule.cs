using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ModalidadReabastecimientoForzadoValidationRule : IValidationRule
    {
        protected readonly string _modalidad;
        protected readonly string _porcentaje;

        public ModalidadReabastecimientoForzadoValidationRule(string modalidad, string porcentaje)
        {
            this._modalidad = modalidad;
            this._porcentaje = porcentaje;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_modalidad == EstrategiaAlmacenajeReabastecimientoDb.FORZADO && string.IsNullOrEmpty(_porcentaje))
                errors.Add(new ValidationError("General_Sec0_Error_Error102"));
            else if(_modalidad != EstrategiaAlmacenajeReabastecimientoDb.FORZADO && !string.IsNullOrEmpty(_porcentaje))
                errors.Add(new ValidationError("General_Sec0_Error_Error103"));

            return errors;
        }
    }
}
