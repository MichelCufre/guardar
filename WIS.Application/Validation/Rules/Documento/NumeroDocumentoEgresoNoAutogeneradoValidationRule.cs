using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class NumeroDocumentoEgresoNoAutogeneradoValidationRule : IValidationRule
    {
        protected readonly string _tipoDocumento;
        protected readonly string _numeroValue;
        protected readonly IUnitOfWork _uow;

        public NumeroDocumentoEgresoNoAutogeneradoValidationRule(string tipoValue, string numeroValue, IUnitOfWork uow)
        {
            this._tipoDocumento = tipoValue;
            this._numeroValue = numeroValue;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var documentoExistente = this._uow.DocumentoRepository.GetEgreso(this._numeroValue, this._tipoDocumento);

            if (documentoExistente != null)
            {
                errors.Add(new ValidationError("EXP052_Sec0_Error_EgresoExistente"));
            }

            return errors;
        }
    }
}
