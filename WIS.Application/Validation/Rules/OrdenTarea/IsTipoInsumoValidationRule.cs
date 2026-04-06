using System.Collections.Generic;
using WIS.Domain.OrdenTarea.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    public class IsTipoInsumoValidationRule : IValidationRule
    {
        protected readonly string _tipo;

        public IsTipoInsumoValidationRule(string tipo)
        {
            this._tipo = tipo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._tipo != OrdenTareaDb.TipoInsumo)
                errors.Add(new ValidationError("ORT020_Sec0_Error_NoEsTipoInsumo"));

            return errors;
        }
    }
}
