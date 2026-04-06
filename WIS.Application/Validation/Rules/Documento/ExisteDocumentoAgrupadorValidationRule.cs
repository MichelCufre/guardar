using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class ExisteDocumentoAgrupadorValidationRule : IValidationRule
    {
        protected readonly string _tipoDocumento;
        protected readonly string _numeroValue;
        protected readonly IUnitOfWork _uow;

        public ExisteDocumentoAgrupadorValidationRule(string tipoValue, string numeroValue, IUnitOfWork uow)
        {
            this._tipoDocumento = tipoValue;
            this._numeroValue = numeroValue;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var agrupadorExistente = this._uow.DocumentoRepository.GetAgrupador(this._numeroValue, this._tipoDocumento);

            if (agrupadorExistente != null)
            {
                errors.Add(new ValidationError("DOC320_Sec0_Not_Error02"));
            }

            return errors;
        }
    }
}
