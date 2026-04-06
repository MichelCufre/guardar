using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdOndaNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdOndaNoExistenteValidationRule(IUnitOfWork uow, string idOnda)
        {
            this._value = idOnda;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.OndaRepository.AnyOnda(short.Parse(_value)))
                errors.Add(new ValidationError("General_Sec0_Error_IdOndaExistente"));

            return errors;
        }
    }
}
