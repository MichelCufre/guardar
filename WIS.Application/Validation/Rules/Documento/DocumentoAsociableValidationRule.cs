using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class DocumentoAsociableValidationRule : IValidationRule
    {
        protected readonly string _tpNuDocumento;
        protected readonly IUnitOfWork _uow;

        public DocumentoAsociableValidationRule(string tpNuDocumento, IUnitOfWork uow)
        {
            this._tpNuDocumento = tpNuDocumento;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._tpNuDocumento))
            {
                var index = this._tpNuDocumento.IndexOf("_");
                var nuDocumento = this._tpNuDocumento.Substring(index + 1);
                var tpDocumento = this._tpNuDocumento.Substring(0, index);

                if (!this._uow.DocumentoRepository.IsDocumentoAsociable(tpDocumento, nuDocumento))
                    errors.Add(new ValidationError("General_Sec0_Error_DocumentoNoAsociable"));
            }

            return errors;
        }
    }
}
