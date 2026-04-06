using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteAjusteStockValidationRule : IValidationRule
    {
        protected readonly string _nroAjusteStock;
        protected readonly IUnitOfWork _uow;

        public ExisteAjusteStockValidationRule(IUnitOfWork uow, string nroAjusteStock)
        {
            this._nroAjusteStock = nroAjusteStock;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (!this._uow.AjusteRepository.AnyAjuste(int.Parse(this._nroAjusteStock)))
                errors.Add(new ValidationError("General_Sec0_Error_Error05"));

            return errors;
        }
    }
}
