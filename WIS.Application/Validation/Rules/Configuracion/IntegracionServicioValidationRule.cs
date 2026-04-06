using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Configuracion
{
    public class IntegracionServicioValidationRule : IValidationRule
    {
        protected IUnitOfWork _uow;
        protected int _value;

        public IntegracionServicioValidationRule(IUnitOfWork uow, int nroIntegracion)
        {
            this._uow = uow;
            this._value = nroIntegracion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.IntegracionServicioRepository.AnyIntegracionServicio(_value))
                errors.Add(new ValidationError("AUT100Modal_Sec0_Error_IntegracionInexistente"));

            return errors;
        }
    }
}
