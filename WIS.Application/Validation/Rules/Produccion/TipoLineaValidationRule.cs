using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class TipoLineaValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public TipoLineaValidationRule(IUnitOfWork uow, string Value)
        {
            this._uow = uow;
            this._value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            var tipoLinea = this._uow.DominioRepository.GetDominios(TipoLineaProduccion.DominioTipoLinea);

            if (!tipoLinea.Any(d => d.Id == this._value))
                errors.Add(new ValidationError("General_Sec0_Error_TipoLineaNoExiste"));

            return errors;
        }
    }
}
