using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Evento
{
    public class NoExisteEventoTemplateValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _nuEvento;
        protected readonly string _tpNotificacion;
        protected readonly string _cdPlantilla;

        public NoExisteEventoTemplateValidationRule(IUnitOfWork uow, string nuEvento, string tpNotificacion, string cdPlantilla)
        {
            _nuEvento = nuEvento;
            _tpNotificacion = tpNotificacion;
            _cdPlantilla = cdPlantilla;
            _uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.EventoRepository.AnyTemplate(int.Parse(_nuEvento), _tpNotificacion, _cdPlantilla))
                errors.Add(new ValidationError("General_Sec0_Error_EventoTemplateNoExiste"));


            return errors;
        }
    }
}
