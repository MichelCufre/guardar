using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteEnvaseValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _tipo;
        protected readonly IUnitOfWork _uow;

        public ExisteEnvaseValidationRule(IUnitOfWork uow, string value, string tipo)
        {
            this._value = value;
            this._tipo = tipo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(_tipo))
            {
                errors.Add(new ValidationError("STO210_Op_Error_TipoEnvaseRequerido"));
            }
            else if (_uow.EnvaseRepository.AnyEnvase(this._value, this._tipo))
            {
                errors.Add(new ValidationError("STO210_Op_Error_EnvaseYaExiste"));
            }

            return errors;
        }
    }
}
