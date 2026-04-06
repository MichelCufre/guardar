using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    public class ExisteCodigoComponenteValidationRule : IValidationRule
    {
        protected readonly string _cdFacturacion;
        protected readonly string _nuComponente;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoComponenteValidationRule(IUnitOfWork uow, string cdFacturacion, string nuComponente)
        {
            this._cdFacturacion = cdFacturacion;
            this._nuComponente = nuComponente;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.FacturacionRepository.AnyFacturacion(this._nuComponente, this._cdFacturacion))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoComponenteNoExiste"));

            return errors;
        }
    }
}
