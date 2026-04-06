using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdExternoPredioUnicolValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdExternoPredioUnicolValidationRule(IUnitOfWork uow, string idExterno)
        {
            this._value = idExterno;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.PredioRepository.AnyIdExternoPredio(_value))
                errors.Add(new ValidationError("REG705_Sec0_Error_IdExternoYaExiste"));

            return errors;
        }
    }
}
