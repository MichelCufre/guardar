using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Evento
{
    public class ExisteEventoInstanciaValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public ExisteEventoInstanciaValidationRule(IUnitOfWork uow, string value)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

           if (!_uow.EventoRepository.AnyInstancia(int.Parse(_value)))
                errors.Add(new ValidationError("EVT040_Sec0_Error_InstanciaNoExiste"));

            return errors;
        }
    }
}
