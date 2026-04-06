using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdZonaUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdZonaUbicacionValidationRule(IUnitOfWork uow, string zona)
        {
            this._value = zona;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._value))
                if (!_uow.ZonaUbicacionRepository.AnyZona(this._value))
                    errors.Add(new ValidationError("REG040_Frm1_Error_ZonaExiste", new List<string> { this._value }));

            return errors;
        }

    }
}
