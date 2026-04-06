using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class TipoDocumentoHabilitadoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;

        public TipoDocumentoHabilitadoValidationRule(IUnitOfWork uow, string value)
        {
            this._uow = uow;
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var tpDocumento = this._uow.DocumentoTipoRepository.GetTipoDocumento(this._value);

            if (!tpDocumento.Habilitado)
                errors.Add(new ValidationError("STO800_Sec0_Error_TipoDocumentoInhabilitado"));

            return errors;
        }
    }
}