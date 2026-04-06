using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DominioValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;
        public DominioValidationRule(string value, IUnitOfWork uow)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._value))
            {
                if (!_uow.DominioRepository.ExisteDominio(this._value))
                    errors.Add(new ValidationError("General_Sec0_Error_Dominio"));
            }

            return errors;
        }
    }
}
