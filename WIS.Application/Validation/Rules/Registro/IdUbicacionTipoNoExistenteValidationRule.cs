using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdUbicacionTipoNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdUbicacionTipoNoExistenteValidationRule(IUnitOfWork uow, string idUbicacionTipo)
        {
            this._value = idUbicacionTipo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_value, out short valor))
                if (!_uow.UbicacionTipoRepository.AnyUbicacionTipo(valor))
                    errors.Add(new ValidationError("General_Sec0_Error_IdUbicacionTipoNoExistente"));

            return errors;
        }
    }
}
