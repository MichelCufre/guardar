using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class TipoIngresoValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public TipoIngresoValidationRule(IUnitOfWork uow, string Value)
        {
            this._uow = uow;
            this._value = Value;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            var tiposIngreso = this._uow.DominioRepository.GetDominios(CodigoDominioDb.TipoIngresoProduccion);

            if (!tiposIngreso.Any(d => d.Id == this._value))
                errors.Add(new ValidationError("General_Sec0_Error_TipoIngresoPrdNoExiste"));

            return errors;
        }
    }
}
