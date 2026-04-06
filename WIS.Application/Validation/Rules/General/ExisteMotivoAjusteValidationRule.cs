using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteMotivoAjusteValidationRule : IValidationRule
    {
        protected readonly string _cdMotivoAjuste;
        protected readonly IUnitOfWork _uow;

        public ExisteMotivoAjusteValidationRule(IUnitOfWork uow, string cdMotivoAjuste)
        {
            this._cdMotivoAjuste = cdMotivoAjuste;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (!this._uow.AjusteRepository.AnyMotivoAjuste(this._cdMotivoAjuste))
                errors.Add(new ValidationError("El motivo de ajuste ingresado no exite."));

            return errors;
        }
    }
}
