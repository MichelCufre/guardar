using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class NumeroDocumentoIngresoNoAutogeneradoValidationRule : IValidationRule
    {
        protected readonly string _tipoDocumento;
        protected readonly string _numeroValue;
        protected readonly IUnitOfWork _uow;

        public NumeroDocumentoIngresoNoAutogeneradoValidationRule(string tipoValue, string numeroValue, IUnitOfWork uow)
        {
            this._tipoDocumento = tipoValue;
            this._numeroValue = numeroValue;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var documentoExistente = this._uow.DocumentoRepository.GetIngreso(this._numeroValue, this._tipoDocumento);

            if (documentoExistente != null)
            {
                errors.Add(new ValidationError("DOC080_Sec0_Error_Error05"));
            }

            return errors;
        }
    }
}
