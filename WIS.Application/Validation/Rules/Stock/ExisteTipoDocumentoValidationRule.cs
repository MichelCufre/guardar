using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class ExisteTipoDocumentoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;

        public ExisteTipoDocumentoValidationRule(IUnitOfWork uow, string value)
        {
            this._uow = uow;
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.DocumentoTipoRepository.AnyTipoDocumento(this._value))
                errors.Add(new ValidationError("STO800_Sec0_Error_TipoDocumentoNoExiste"));

            return errors;
        }
    }
}
