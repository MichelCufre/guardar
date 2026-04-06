using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Evento
{
    public class ExisteEventoValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public ExisteEventoValidationRule(IUnitOfWork uow, string value)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

           if (!_uow.EventoRepository.AnyEvento(int.Parse(_value)))
                errors.Add(new ValidationError("EVT040_Sec0_Error_EventoNoExiste"));

            return errors;
        }
    }
}
