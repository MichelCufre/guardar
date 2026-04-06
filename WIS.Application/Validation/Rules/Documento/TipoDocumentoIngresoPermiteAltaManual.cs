using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class TipoDocumentoIngresoPermiteAltaManual : IValidationRule
    {
        protected readonly string _tipoValue;
        protected readonly IUnitOfWork _uow;

        public TipoDocumentoIngresoPermiteAltaManual(string tipoValue, IUnitOfWork uow)
        {
            this._tipoValue = tipoValue;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.DocumentoTipoRepository.GetDocumentosIngresoHabilitados().Any(a => a.TipoDocumento == this._tipoValue))
                errors.Add(new ValidationError("General_Sec0_Error_Error37"));

            return errors;
        }
    }
}
