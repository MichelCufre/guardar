using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    class ExisteCodigoDeTarea : IValidationRule
    {
        protected readonly string _idCodigo;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoDeTarea(IUnitOfWork uow, string valueCodigo)
        {
            this._idCodigo = valueCodigo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.TareaRepository.AnyTarea(this._idCodigo))
                errors.Add(new ValidationError("General_ORT090_Error_TareaNoExiste"));

            return errors;
        }
    }
}
