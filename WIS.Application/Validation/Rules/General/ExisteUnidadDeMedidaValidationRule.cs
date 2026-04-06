using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteUnidadDeMedidaValidationRule : IValidationRule
    {
        protected readonly string _unidad;
        protected readonly IUnitOfWork _uow;

        public ExisteUnidadDeMedidaValidationRule(IUnitOfWork uow, string unidad)
        {
            this._unidad = unidad;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            //if (!this._context.T_UNIDADE_MEDIDA.Any(d => d.CD_UNIDADE_MEDIDA == this._unidad))
            //    errors.Add(new ValidationError("General_Sec0_Error_Error23"));

            return errors;
        }
    }
}
