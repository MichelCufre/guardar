using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteDespachanteValidationRule : IValidationRule
    {
        protected readonly string _despachanteValue;
        protected readonly IUnitOfWork _uow;

        public ExisteDespachanteValidationRule(IUnitOfWork uow, string despachanteValue)
        {
            this._despachanteValue = despachanteValue;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var cdDespachante = int.Parse(this._despachanteValue);

            var errors = new List<IValidationError>();

            //if (!this._context.T_DESPACHANTE.Any(d => d.CD_DESPACHANTE == cdDespachante))
            //    errors.Add(new ValidationError("General_Sec0_Error_Error18"));

            return errors;
        }
    }
}
