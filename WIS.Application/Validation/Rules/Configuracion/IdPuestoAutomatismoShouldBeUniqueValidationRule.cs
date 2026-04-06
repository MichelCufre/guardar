using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Configuracion
{
    public class IdPuestoAutomatismoShouldBeUniqueValidationRule : IValidationRule
    {
        protected IUnitOfWork _uow;
        protected string _value;
        protected string _automatismoPuesto;

        public IdPuestoAutomatismoShouldBeUniqueValidationRule(IUnitOfWork uow, string value, string automatismoPuesto)
        {
            _uow = uow;
            _value = value;
            _automatismoPuesto = automatismoPuesto;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(_automatismoPuesto) && _uow.AutomatismoPuestoRepository.AnyIdPuesto(_value))
                errors.Add(new ValidationError("AUT100Puestos_Sec0_Error_IdPuestoShouldBeUnique"));
            else if (int.TryParse(_automatismoPuesto, out int automatismoPuesto))
            {
                if (_uow.AutomatismoPuestoRepository.AnyIdPuesto(_value, automatismoPuesto))
                    errors.Add(new ValidationError("AUT100Puestos_Sec0_Error_IdPuestoShouldBeUnique"));
            }
            return errors;
        }
    }
}
