using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class ExisteAgendaValidationRule : IValidationRule
    {
        protected readonly int _agenda;
        protected readonly IUnitOfWork _uow;

        public ExisteAgendaValidationRule(IUnitOfWork uow, int agenda)
        {
            this._agenda = agenda;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.AgendaRepository.AnyAgenda(this._agenda))
                errors.Add(new ValidationError("General_Sec0_Error_AgendaNoExiste"));

            return errors;
        }
    }
}
