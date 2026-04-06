using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdUbicacionAreaNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdUbicacionAreaNoExistenteValidationRule(IUnitOfWork uow, string codigoArea)
        {
            this._value = codigoArea;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_value, out short valor))
                if (!_uow.UbicacionAreaRepository.AnyUbicacionArea(valor))
                    errors.Add(new ValidationError("General_Sec0_Error_CodigoUbicacionAreaNoExistente"));

            return errors;
        }
    }
}
