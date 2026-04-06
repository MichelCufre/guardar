using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteDominioFacturacionValidationRule : IValidationRule
    {
        protected readonly string _dominio;
        protected readonly IUnitOfWork _uow;

        public ExisteDominioFacturacionValidationRule(IUnitOfWork uow, string dominio)
        {
            this._dominio = dominio;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.DominioRepository.ExisteDetalleDominio(FacturacionDb.FacturaProductoCompra, this._dominio))
                errors.Add(new ValidationError("REG009_Sec0_Error_Er008_NoExisteDominioComponente"));

            return errors;
        }
    }
}