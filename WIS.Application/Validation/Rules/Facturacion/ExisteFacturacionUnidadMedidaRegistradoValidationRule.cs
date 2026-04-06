using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    public class ExisteFacturacionUnidadMedidaRegistradoValidationRule : IValidationRule
    {
        protected readonly string _unidadMedida;
        protected readonly IUnitOfWork _uow;

        public ExisteFacturacionUnidadMedidaRegistradoValidationRule(IUnitOfWork uow, string unidadMedida)
        {
            this._unidadMedida = unidadMedida;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.FacturacionRepository.AnyFacturacionUnidadMedida(this._unidadMedida))
                errors.Add(new ValidationError("General_Sec0_Error_FacturacionUnidadMedidaExiste"));

            return errors;
        }
    }
}
