using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteEmpresaValidationRule : IValidationRule
    {
        protected readonly string _valueEmpresa;
        protected readonly IUnitOfWork _uow;

        public ExisteEmpresaValidationRule(IUnitOfWork uow, string idEmpresa)
        {
            this._valueEmpresa = idEmpresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var cdEmpresa = int.Parse(this._valueEmpresa);

            var errors = new List<IValidationError>();

            if(!_uow.EmpresaRepository.AnyEmpresa(cdEmpresa))
                errors.Add(new ValidationError("General_Sec0_Error_Error17"));

            return errors;
        }
    }
}
