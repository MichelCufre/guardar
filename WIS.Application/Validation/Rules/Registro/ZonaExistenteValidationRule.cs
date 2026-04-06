using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ZonaExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public ZonaExistenteValidationRule(IUnitOfWork uow, string zona)
        {
            this._value = zona;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ZonaRepository.AnyZona(this._value))
                errors.Add(new ValidationError("REG104_frm1_Error_ZonaExistente", new List<string> { this._value }));

            return errors;
        }
    }
}
