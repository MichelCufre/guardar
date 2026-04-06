using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteTipoAlmacenajeSeguroValidationRule : IValidationRule
    {
        protected readonly string _almacenajeSeguroValue;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoAlmacenajeSeguroValidationRule(IUnitOfWork uow, string almacenajeSeguroValue)
        {
            this._almacenajeSeguroValue = almacenajeSeguroValue;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var tpAlmacenajeSeguro = short.Parse(this._almacenajeSeguroValue);

            var errors = new List<IValidationError>();

            //if (!this._context.T_TIPO_ALMACENAJE_SEGURO.Any(a => a.TP_ALMACENAJE_Y_SEGURO == tpAlmacenajeSeguro))
            //    errors.Add(new ValidationError("General_Sec0_Error_Error20"));

            return errors;
        }
    }
}
