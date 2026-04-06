using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class ZonaNoExisteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public ZonaNoExisteValidationRule(IUnitOfWork uow, string zona)
        {
            _value = zona;
            _uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.ZonaRepository.AnyZona(_value))
                errors.Add(new ValidationError("PRE100_frm1_Error_ZonaNoExiste", new List<string> { _value }));

            return errors;
        }
    }
}
