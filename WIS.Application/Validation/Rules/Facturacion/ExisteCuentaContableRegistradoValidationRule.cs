using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    public class ExisteCuentaContableRegistradoValidationRule : IValidationRule
    {
        protected readonly string _cuenta;
        protected readonly IUnitOfWork _uow;

        public ExisteCuentaContableRegistradoValidationRule(IUnitOfWork uow, string valueCuenta)
        {
            this._cuenta = valueCuenta;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.CuentaContableRepository.ExisteCuentaContable(this._cuenta))
                errors.Add(new ValidationError("General_Sec0_Error_CuentaContableExiste"));

            return errors;
        }
    }
}
