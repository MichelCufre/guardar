using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class MotivoConsumoPermitidoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _motivo;
        protected readonly bool _consumible;

        public MotivoConsumoPermitidoValidationRule(IUnitOfWork uow, string motivo, bool consumible)
        {
            this._uow = uow;
            this._motivo = motivo;
            this._consumible = consumible;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (!_consumible && _motivo== TipoIngresoProduccion.MOT_CONS_ADS)
                errors.Add(new ValidationError("PRD113_grid1_Error_MotivoInvalido"));

            return errors;
        }
    }
}
