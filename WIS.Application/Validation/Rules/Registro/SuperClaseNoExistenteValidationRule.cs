using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class SuperClaseNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public SuperClaseNoExistenteValidationRule(IUnitOfWork uow, string codigoSubClase)
        {
            this._value = codigoSubClase;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.ClaseRepository.AnySuperClase(_value))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoSuperClaseUbicacionNoExiste"));

            return errors;
        }
    }
}
