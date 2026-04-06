using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    public class OrdenesDistintasTareaAmigableValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _nuOrdenOrigen;
        protected readonly int _nuOrdenTareaActiva;

        public OrdenesDistintasTareaAmigableValidationRule(IUnitOfWork uow, int nuOrdenOrigen, int nuOrdenTareaActiva)
        {
            this._uow = uow;
            this._nuOrdenTareaActiva = nuOrdenTareaActiva;
            this._nuOrdenOrigen = nuOrdenOrigen;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._nuOrdenOrigen != this._nuOrdenTareaActiva)
                errors.Add(new ValidationError("General_ORT090_Error_OrdenesDistintas"));

            return errors;
        }
    }
}
