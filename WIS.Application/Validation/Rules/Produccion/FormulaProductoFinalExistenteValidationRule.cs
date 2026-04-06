using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class FormulaProductoFinalExistenteValidationRule : IValidationRule
    {
        protected readonly string _formula;
        protected readonly string _producto;
        protected readonly IUnitOfWork _uow;

        public FormulaProductoFinalExistenteValidationRule(IUnitOfWork uow, string formula, string producto)
        {
            this._uow = uow;
            this._formula = formula;
            this._producto = producto;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (this._uow.FormulaRepository.AnyFormulaSalida(this._formula, this._producto))
                errors.Add(new ValidationError("PRD100_Sec0_error_ProductoSalidaDuplicado"));

            return errors;
        }
    }
}
