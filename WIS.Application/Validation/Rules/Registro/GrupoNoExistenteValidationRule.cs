using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class GrupoNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public GrupoNoExistenteValidationRule(IUnitOfWork uow, string value)
        {
            this._value = value.ToUpper();
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.GrupoRepository.AnyGrupo(_value))
                errors.Add(new ValidationError("REG300_msg_Error_GrupoNoExiste", new List<string>() { _value }));

            return errors;
        }
    }
}
