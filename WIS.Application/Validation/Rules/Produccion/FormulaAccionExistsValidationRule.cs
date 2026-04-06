using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class FormulaAccionExistsValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public FormulaAccionExistsValidationRule(IUnitOfWork uow, string Value)
        {
            this._uow = uow;
            this._value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            short id = short.Parse(this._value);

            if (!_uow.FormulaAccionRepository.AnyFormulaAccion(id))
                errors.Add(new ValidationError("PRD100_Sec0_error_AccionNoExiste"));

            return errors;
        }
    }
}
