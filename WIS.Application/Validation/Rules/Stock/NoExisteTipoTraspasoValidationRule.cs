using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class NoExisteTipoTraspasoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;

        public NoExisteTipoTraspasoValidationRule(IUnitOfWork uow, string value)
        {
            this._uow = uow;
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var tiposTraspaso = this._uow.TraspasoEmpresasRepository.GetTiposTraspaso();

            if (!tiposTraspaso.Any(x => x.Key == this._value))
                errors.Add(new ValidationError("STO820_Sec0_Error_TipoTraspasoNoExiste"));

            return errors;
        }
    }
}
