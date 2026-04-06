using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class DescripcionOndaNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;


        public DescripcionOndaNoExistenteValidationRule(IUnitOfWork uow, string descripcionOnda)
        {
            this._value = descripcionOnda;
            this._uow = uow;

        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.OndaRepository.AnyOnda(_value))
                errors.Add(new ValidationError("General_Sec0_Error_DescripcionOndaExistente"));

            return errors;
        }
    }
}