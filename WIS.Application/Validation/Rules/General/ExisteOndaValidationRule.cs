using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteOndaValidationRule : IValidationRule
    {
        protected readonly string _valueOnda;
        protected readonly string _valueEmpresa;
        protected readonly IUnitOfWork _uow;

        public ExisteOndaValidationRule(IUnitOfWork uow, string valueOnda, string valueEmpresa)
        {
            this._valueEmpresa = valueEmpresa;
            this._valueOnda = valueOnda;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (int.TryParse(this._valueEmpresa, out int cdEmpresa))
            {

                if (!_uow.LiberacionRepository.AnyOnda(cdEmpresa,short.Parse(_valueOnda)))
                    errors.Add(new ValidationError("General_Sec0_Error_Error62"));
            }
            else
            {
                errors.Add(new ValidationError("General_Sec0_Error_EmpresaNecesaria"));
            }
            return errors;
        }
    }
}
