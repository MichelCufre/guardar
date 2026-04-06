using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteViaActivaValidationRule : IValidationRule
    {
        protected readonly string _valueVia;
        protected readonly IUnitOfWork _uow;

        public ExisteViaActivaValidationRule(IUnitOfWork uow, string valueVia)
        {
            this._valueVia = valueVia;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            //if (!this._context.T_VIA.Any(d => d.CD_VIA == this._valueVia))
            //    errors.Add(new ValidationError("General_Sec0_Error_Error24"));

            return errors;
        }
    }
}
