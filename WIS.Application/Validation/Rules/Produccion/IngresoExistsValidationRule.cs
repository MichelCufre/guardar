using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class IngresoExistsValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IngresoExistsValidationRule(IUnitOfWork uow, string Value)
        {
            this._uow = uow;
            this._value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (!this._uow.ProduccionRepository.AnyIngreso(this._value))
                errors.Add(new ValidationError("KIT130_Sec0_Error_WB002_NroingresoProNoExiste"));

            return errors;
        }
    }
}
