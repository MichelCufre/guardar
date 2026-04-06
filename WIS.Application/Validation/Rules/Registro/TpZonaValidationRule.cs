using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class TpZonaValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public TpZonaValidationRule(IUnitOfWork uow, string zona)
        {
            this._value = zona;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.ZonaUbicacionRepository.AnyTpZona(this._value))
                errors.Add(new ValidationError("REG070_Frm1_Error_TpZonaNoExiste", new List<string> { this._value }));

            return errors;
        }
    }
}
