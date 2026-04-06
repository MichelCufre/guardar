using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class TipoAlmacenajeSeguroExistenteValidationRule : IValidationRule
    {
        protected readonly short _value;
        protected readonly IUnitOfWork _uow;

        public TipoAlmacenajeSeguroExistenteValidationRule(IUnitOfWork uow, short tipo)
        {
            this._value = tipo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.TipoAlmacenajeSeguroRepository.AnyTipoAlmacenajeYSeguro(_value))
                errors.Add(new ValidationError("General_Sec0_Error_TipoAlmacenajeYSeguroNoExistente"));

            return errors;
        }

    }
}
