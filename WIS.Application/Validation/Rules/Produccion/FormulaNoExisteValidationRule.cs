using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class FormulaNoExisteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public FormulaNoExisteValidationRule(IUnitOfWork uow, string Value)
        {
            this._uow = uow;
            this._value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (!this._uow.FormulaRepository.AnyFormula(this._value))
                errors.Add(new ValidationError("General_Sec0_Error_FormulaNoExiste"));

            return errors;
        }
    }
}
