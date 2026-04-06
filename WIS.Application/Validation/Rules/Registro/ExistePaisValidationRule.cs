using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExistePaisValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public ExistePaisValidationRule(IUnitOfWork uow, string codigoPais)
        {
            this._value = codigoPais;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.PaisRepository.AnyPais(_value))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoPaisNoExistente"));

            return errors;
        }
    }
}