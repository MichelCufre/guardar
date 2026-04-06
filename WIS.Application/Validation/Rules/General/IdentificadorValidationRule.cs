using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Logic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class IdentificadorValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;

        public IdentificadorValidationRule(IUnitOfWork uow, string identificador)
        {
            _uow = uow;
            _value = identificador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (LIdentificador.ContieneCaracteresNoPermitidos(_uow, _value) || this._value == ManejoIdentificadorDb.IdentificadorAuto)
                errors.Add(new ValidationError("General_Sec0_Error_IdentificadorInvalido"));

            return errors;
        }
    }
}