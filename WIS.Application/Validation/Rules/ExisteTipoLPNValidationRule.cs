using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteTipoLPNValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;
        public ExisteTipoLPNValidationRule(IUnitOfWork uow, string value)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.ManejoLpnRepository.AnyTipoLpn(this._value))
                errors.Add(new ValidationError("General_Sec0_Error_TipoLPNNoExiste"));
            
            return errors;
        }
    }
}
