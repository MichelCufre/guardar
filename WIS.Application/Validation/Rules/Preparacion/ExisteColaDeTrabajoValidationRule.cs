using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class ExisteColaDeTrabajoValidationRule : IValidationRule
    {
        protected readonly string _colaDeTrabajo;
        protected readonly IUnitOfWork _uow;

        public ExisteColaDeTrabajoValidationRule(IUnitOfWork uow, string colaDeTrabajo)
        {
            this._colaDeTrabajo = colaDeTrabajo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._colaDeTrabajo))
            {
                if (this._uow.ColaDeTrabajoRepository.AnyColaDeTrabajo(int.Parse(this._colaDeTrabajo)))
                    errors.Add(new ValidationError("PRE810_Sec0_Error_ColaDeTrabajoRegistrada"));
            }

            return errors;
        }
    }
}
