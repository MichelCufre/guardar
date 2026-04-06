using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    public class ExisteTareaValidationRule : IValidationRule
    {
        protected readonly string _tarea;
        protected readonly IUnitOfWork _uow;

        public ExisteTareaValidationRule(IUnitOfWork uow, string valueTarea)
        {
            this._tarea = valueTarea;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.TareaRepository.AnyTarea(this._tarea))
                errors.Add(new ValidationError("General_Sec0_Error_TareaExiste"));

            return errors;
        }
    }
}
