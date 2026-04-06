using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdUbicacionAreaNoMantenibleValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdUbicacionAreaNoMantenibleValidationRule(IUnitOfWork uow, string codigoArea)
        {
            this._value = codigoArea;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_value, out short valor))
                if (!_uow.UbicacionAreaRepository.AnyUbicacionAreaMantenible(valor))
                    errors.Add(new ValidationError("General_Sec0_Error_CodigoUbicacionAreaNoPermiteEdicion"));

            return errors;
        }
    }
}
