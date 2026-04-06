using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteMonedaValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _moneda;

        public ExisteMonedaValidationRule(IUnitOfWork uow, string value)
        {
            this._moneda = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
                        
            if(!this._uow.MonedaRepository.ExisteMoneda(this._moneda))
                errors.Add(new ValidationError("General_Sec0_Error_Error19"));

            return errors;
        }
    }
}
