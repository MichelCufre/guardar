using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class TipoTraspasoAsociadoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;
        protected readonly long _config;

        public TipoTraspasoAsociadoValidationRule(IUnitOfWork uow, string value, long config)
        {
            this._uow = uow;
            this._value = value;
            this._config = config;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var tiposTraspaso = this._uow.TraspasoEmpresasRepository.GetTiposTraspaso(this._config);

            if (!tiposTraspaso.Any(x => x.Key == this._value))
                errors.Add(new ValidationError("STO820_Sec0_Error_TipoTraspasoNoAsociado"));

            return errors;
        }
    }
}
