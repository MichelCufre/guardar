using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteDominioValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _codigo;
        protected readonly IUnitOfWork _uow;

        public ExisteDominioValidationRule(string value, IUnitOfWork uow, string codigo = null)
        {
            this._value = value;
            this._codigo = codigo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_codigo == null)
            {
                if (!_uow.DominioRepository.ExisteDetalleDominio(_value))
                    errors.Add(new ValidationError("General_Db_Error_NoExiste"));
            }
            else
            {
                if (!_uow.DominioRepository.ExisteDetalleDominioValor(_codigo, _value))
                    errors.Add(new ValidationError("General_Db_Error_NoExiste"));
            }

            return errors;
        }
    }
}
