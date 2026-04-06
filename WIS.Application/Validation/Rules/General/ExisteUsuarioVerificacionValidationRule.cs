using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    class ExisteUsuarioVerificacionValidationRule : IValidationRule
    {
        protected readonly string _valueLoginname;
        protected readonly IUnitOfWork _uow;

        public ExisteUsuarioVerificacionValidationRule(IUnitOfWork uow, string valueLoginname)
        {
            this._valueLoginname = valueLoginname;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.SecurityRepository.AnyUsuario(this._valueLoginname))
                errors.Add(new ValidationError("General_ORT090_Error_UsuarioNoExiste"));

            return errors;
        }
    }
}
