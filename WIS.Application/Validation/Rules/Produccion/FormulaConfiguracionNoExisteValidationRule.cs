using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class FormulaConfiguracionNoExisteValidationRule : IValidationRule
    {
        protected readonly string _formula;
        protected readonly string _accion;
        protected readonly IUnitOfWork _uow;

        public FormulaConfiguracionNoExisteValidationRule(IUnitOfWork uow, string formula, string accion)
        {
            this._uow = uow;
            this._formula = formula;
            this._accion = accion;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            int accion = int.Parse(this._accion);

            if (this._uow.FormulaRepository.AnyFormulaConfiguracion(this._formula, accion))
                errors.Add(new ValidationError("PRD100_Sec0_error_AccionDuplicada"));

            return errors;
        }
    }
}
