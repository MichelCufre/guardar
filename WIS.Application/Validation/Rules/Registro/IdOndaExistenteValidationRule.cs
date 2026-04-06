using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdOndaExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdOndaExistenteValidationRule(IUnitOfWork uow, string idOnda)
        {
            this._value = idOnda;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            short id = short.Parse(_value);

            if (id == 0)
            {
                errors.Add(new ValidationError("General_Sec0_Error_IdOndaReservado"));
            }
            else
            {
                if (!_uow.OndaRepository.AnyOnda(id))
                    errors.Add(new ValidationError("General_Sec0_Error_IdOndaNoExistente"));
            }

            return errors;
        }
    }
}