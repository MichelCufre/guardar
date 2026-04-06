using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    class ExisteCodigoComponenteEnFactura : IValidationRule
    {
        protected readonly string _idCodigo;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoComponenteEnFactura(IUnitOfWork uow, string valueCodigo)
        {
            this._idCodigo = valueCodigo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.FacturacionRepository.AnyFacturacionComponente(this._idCodigo))
                errors.Add(new ValidationError("General_Sec0_Error_ComponenteNoExiste"));

            return errors;
        }
    }
}
