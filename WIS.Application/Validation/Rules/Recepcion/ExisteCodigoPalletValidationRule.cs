
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class ExisteCodigoPalletValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoPalletValidationRule(IUnitOfWork uow, string codigoPallet)
        {
            this._value = codigoPallet;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if(short.TryParse(this._value, out short parsedValue))
                if(!this._uow.FacturacionRepository.ExisteCodigoPallet(parsedValue))
                    errors.Add(new ValidationError("General_Sec0_Error_NoExistePallet"));

            return errors;
        }
    }
}