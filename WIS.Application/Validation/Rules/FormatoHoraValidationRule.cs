using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class FormatoHoraValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public FormatoHoraValidationRule(string value, IUnitOfWork uow)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.AtributoRepository.AnyFormatoHora(_value))
                errors.Add(new ValidationError("General_Db_Error_NoExisteFormatoHora"));


            return errors;
        }
    }
}
