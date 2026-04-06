using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class PredioUsuarioExistenteValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;
        protected readonly int _idUsuario;

        public PredioUsuarioExistenteValidationRule(IUnitOfWork uow, int idUsuario, string value)
        {
            this._uow = uow;
            this._value = value;
            this._idUsuario = idUsuario;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.PredioRepository.AnyPrediosUsuario(this._value, this._idUsuario))
                errors.Add(new ValidationError("General_Sec0_Error_PredioNoAsignadoAUsuario", new List<string>() { this._value }));

            return errors;
        }

    }
}
