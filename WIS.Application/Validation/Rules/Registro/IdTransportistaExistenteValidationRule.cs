using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdTransportistaExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdTransportistaExistenteValidationRule(IUnitOfWork uow, string idTransportista)
        {
            this._value = idTransportista;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.TransportistaRepository.AnyTransportista(int.Parse(_value)))
                errors.Add(new ValidationError("General_Sec0_Error_IdTransportistaNoExistente"));

            return errors;
        }
    }
}
