using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    class FacturaYaTieneComponenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _numeroDeFactura;
        protected readonly IUnitOfWork _uow;
        public FacturaYaTieneComponenteValidationRule(string value, string numeroDeFactura, IUnitOfWork uow)
        {
            this._value = value;
            this._numeroDeFactura = numeroDeFactura;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.FacturacionRepository.AnyProceso(this._value , _numeroDeFactura))
                errors.Add(new ValidationError("General_Sec0_Error_YahayUnPrcoesoConEsaFacturaComponente"));

            return errors;
        }
    }

}
