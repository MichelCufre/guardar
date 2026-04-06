using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteUsuarioValidationRule : IValidationRule
    {
        protected readonly string _valueLoginname;
        protected readonly IUnitOfWork _uow;

        public ExisteUsuarioValidationRule(IUnitOfWork uow, string valueLoginname)
        {
            this._valueLoginname = valueLoginname;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.SecurityRepository.AnyUsuario(this._valueLoginname))
                errors.Add(new ValidationError("General_Sec0_Error_YaExisteUsuario"));

            return errors;
        }
    }
}
