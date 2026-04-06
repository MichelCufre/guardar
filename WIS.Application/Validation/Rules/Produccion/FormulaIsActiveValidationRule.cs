using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Produccion.Enums;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class FormulaIsActiveValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public FormulaIsActiveValidationRule(IUnitOfWork uow, string Value)
        {
            this._uow = uow;
            this._value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            var formula = this._uow.FormulaRepository.GetFormula(this._value);

            if (formula.Estado != SituacionDb.Activo)
                errors.Add(new ValidationError("General_Sec0_Error_FormulaNoActiva"));

            return errors;
        }
    }
}
