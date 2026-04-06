
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class ExisteControlAccesoValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public ExisteControlAccesoValidationRule(IUnitOfWork uow, string codigoControlAcceso)
        {
            this._value = codigoControlAcceso;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.ZonaUbicacionRepository.AnyControlAcceso(_value))
                errors.Add(new ValidationError("General_Sec0_Error_NoExisteControlAcceso"));

            return errors;
        }
    }
}
