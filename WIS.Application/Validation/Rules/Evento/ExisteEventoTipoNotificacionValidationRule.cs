using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Evento
{
    public class ExisteEventoTipoNotificacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public ExisteEventoTipoNotificacionValidationRule(IUnitOfWork uow, string value)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            List<DominioDetalle> tiposNotificaciones = this._uow.DominioRepository.GetDominios(CodigoDominioDb.EventoTipoNotificacion);

            if (!tiposNotificaciones.Any(d => d.Valor == this._value))
                errors.Add(new ValidationError("EVT040_Sec0_Error_EventoTipoNotificacionNoExiste"));

            return errors;
        }
    }
}
