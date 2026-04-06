using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    public class ExisteInsumoManipuleoValidationRule : IValidationRule
    {
        protected readonly string _insumoManipuleo;
        protected readonly IUnitOfWork _uow;

        public ExisteInsumoManipuleoValidationRule(IUnitOfWork uow, string valueInsumo)
        {
            this._insumoManipuleo = valueInsumo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.InsumoManipuleoRepository.ExisteInsumoManipuleo(this._insumoManipuleo))
                errors.Add(new ValidationError("General_Sec0_Error_InsumoExiste"));

            return errors;
        }
    }
}
