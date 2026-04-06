using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdPuertaExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdPuertaExistenteValidationRule(IUnitOfWork uow, string idPuerta)
        {
            this._value = idPuerta;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.PuertaEmbarqueRepository.AnyPuertaEmbarque(short.Parse(_value)))
                errors.Add(new ValidationError("General_Sec0_Error_IdPuertaEmbarqueNoExistente"));

            return errors;
        }
    }
}
