using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class EmpresaTieneConfiguracionTraspasoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;

        public EmpresaTieneConfiguracionTraspasoValidationRule(IUnitOfWork uow, string value)
        {
            this._uow = uow;
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.TraspasoEmpresasRepository.IsEmpresaOrigenConfigurada(int.Parse(this._value)))
                errors.Add(new ValidationError("STO800_Sec0_Error_EmpresaYaConfigurada"));

            return errors;
        }
    }
}
