using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class AgendaDocumentableValidationRule : IValidationRule
    {
        protected readonly int? _empresa;
        protected readonly string _agenda;
        protected readonly IUnitOfWork _uow;

        public AgendaDocumentableValidationRule(IUnitOfWork uow,
            int? empresa, 
            string agenda)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._agenda = agenda;
        }


        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._empresa.HasValue
                && !string.IsNullOrEmpty(this._agenda)
                && !this._uow.AgendaRepository.IsAgendaDocumentable(this._empresa.Value, int.Parse(this._agenda)))
                errors.Add(new ValidationError("General_Sec0_Error_AgendaNoDocumentable"));

            return errors;
        }
    }
}
