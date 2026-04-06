using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class FormulaExistsValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public FormulaExistsValidationRule(IUnitOfWork uow, string Value)
        {
            this._uow = uow;
            this._value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (this._uow.FormulaRepository.AnyFormula(this._value))
                errors.Add(new ValidationError("PRD100_form1_error_FormulaDuplicada"));

            return errors;
        }
    }
}
