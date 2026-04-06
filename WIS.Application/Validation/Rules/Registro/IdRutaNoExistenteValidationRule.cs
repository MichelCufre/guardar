using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdRutaNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdRutaNoExistenteValidationRule(IUnitOfWork uow, string idRuta)
        {
            this._value = idRuta;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.RutaRepository.AnyRuta(short.Parse(_value)))
                errors.Add(new ValidationError("General_Sec0_Error_IdRutaExistente"));

            return errors;
        }
    }
}
