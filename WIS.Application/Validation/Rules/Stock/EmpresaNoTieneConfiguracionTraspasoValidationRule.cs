using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class EmpresaNoTieneConfiguracionTraspasoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;

        public EmpresaNoTieneConfiguracionTraspasoValidationRule(IUnitOfWork uow, string value)
        {
            this._uow = uow;
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.TraspasoEmpresasRepository.IsEmpresaOrigenConfigurada(int.Parse(this._value)))
                errors.Add(new ValidationError("STO820_Sec0_Error_EmpresaNoConfigurada"));

            return errors;
        }
    }
}
