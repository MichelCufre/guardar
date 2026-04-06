using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Evento
{
    public class ExisteEventoTemplateValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _nuEvento;
        protected readonly string _tpNotificacion;
        protected readonly string _cdPlantilla;

        public ExisteEventoTemplateValidationRule(IUnitOfWork uow, string nuEvento, string tpNotificacion, string cdPlantilla)
        {
            _nuEvento = nuEvento;
            _tpNotificacion = tpNotificacion;
            _cdPlantilla = cdPlantilla;
            _uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.EventoRepository.AnyTemplate(int.Parse(_nuEvento), _tpNotificacion, _cdPlantilla))
                errors.Add(new ValidationError("General_Sec0_Error_EventoTemplateYaExiste"));


            return errors;
        }
    }
}
