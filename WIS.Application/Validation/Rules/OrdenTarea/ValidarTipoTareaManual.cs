using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.OrdenTarea;
using WIS.Domain.OrdenTarea.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    public class ValidarTipoTareaManual : IValidationRule
    {
        protected readonly string _idCodigo;
        protected readonly IUnitOfWork _uow;

        public ValidarTipoTareaManual(IUnitOfWork uow, string valueCodigo)
        {
            this._idCodigo = valueCodigo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            Tarea tarea = this._uow.TareaRepository.GetTarea(this._idCodigo);
            if (tarea.TipoTarea != OrdenTareaDb.TIPO_TAREA_MANUAL)
                errors.Add(new ValidationError("General_ORT040_Error_TipoTareaInvalido"));

            return errors;
        }
    }
}
