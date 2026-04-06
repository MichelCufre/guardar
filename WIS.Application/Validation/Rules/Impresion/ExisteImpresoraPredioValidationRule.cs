using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteImpresoraPredioValidationRule : IValidationRule
    {
        protected readonly string _predio;
        protected readonly IUnitOfWork _uow;

        public ExisteImpresoraPredioValidationRule(IUnitOfWork uow, string predio)
        {
            this._predio = predio;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (!this._uow.ImpresoraRepository.ExisteImpresora(this._predio))
            {
                errors.Add(new ValidationError("IMP050_Sec0_error_NoHayImpresoraEnEsePredio", new List<string> { this._predio }));
            }

            return errors;
        }
    }
}
