using WIS.Domain.DataModel;
using WIS.Validation;

using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ZonaUbicacionExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public ZonaUbicacionExistenteValidationRule(IUnitOfWork uow, string zona)
        {
            this._value = zona;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ZonaUbicacionRepository.AnyZona(this._value))
                errors.Add(new ValidationError("REG070_Frm1_Error_ZonaExistente", new List<string> { this._value }));

            return errors;
        }
    }
}
