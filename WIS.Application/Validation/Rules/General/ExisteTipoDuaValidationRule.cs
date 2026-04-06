using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteTipoDuaValidationRule : IValidationRule
    {
        protected readonly string _tipoDuaValue;
        protected readonly string _tipoDocumento;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoDuaValidationRule(IUnitOfWork uow,
            string tipoDocumento,
            string tipoDuaValue)
        {
            this._tipoDocumento = tipoDocumento;
            this._tipoDuaValue = tipoDuaValue;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.TipoDuaRepository.AnyTipoDua(this._tipoDocumento, this._tipoDuaValue))
                errors.Add(new ValidationError("General_Sec0_Error_Error21"));

            return errors;
        }
    }
}
