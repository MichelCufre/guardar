using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class ExistePreparacionValidationRule : IValidationRule
    {
        protected readonly int _numero;
        protected readonly IUnitOfWork _uow;

        public ExistePreparacionValidationRule(IUnitOfWork uow, int numero)
        {
            this._numero = numero;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.PreparacionRepository.AnyPreparacion(this._numero))
                errors.Add(new ValidationError("General_Sec0_Error_PreparacionNoExiste"));

            return errors;
        }
    }
}
