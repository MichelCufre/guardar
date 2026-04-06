using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Documento.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class TipoDocumentoOperacionEgresoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;

        public TipoDocumentoOperacionEgresoValidationRule(IUnitOfWork uow, string value)
        {
            this._uow = uow;
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var tpDocumento = this._uow.DocumentoTipoRepository.GetTipoDocumento(this._value);

            if (tpDocumento.TipoOperacion != TipoDocumentoOperacion.EGRESO)
                errors.Add(new ValidationError("STO800_Sec0_Error_TipoOperacionDistintoEgreso"));

            return errors;
        }
    }
}
