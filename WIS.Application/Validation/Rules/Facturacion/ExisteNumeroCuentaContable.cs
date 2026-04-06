using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    class ExisteNumeroCuentaContable : IValidationRule
    {
        protected readonly string _idCodigo;
        protected readonly IUnitOfWork _uow;

        public ExisteNumeroCuentaContable(IUnitOfWork uow, string valueCodigo)
        {
            this._idCodigo = valueCodigo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.FacturacionRepository.AnyCuentaContable(this._idCodigo))
                errors.Add(new ValidationError("General_Sec0_Error_CuentaContableNoExiste"));

            return errors;
        }
    }
}
