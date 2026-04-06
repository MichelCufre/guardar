using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class ExisteTipoExpedicionValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _tipoExpedicion;

        public ExisteTipoExpedicionValidationRule(IUnitOfWork uow, string tipoExpedicion)
        {
            this._uow = uow;
            this._tipoExpedicion = tipoExpedicion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.PedidoRepository.AnyTipoExpedicion(this._tipoExpedicion))
                errors.Add(new ValidationError("General_Sec0_Error_Er091_TipoExpedicionNoExiste"));

            return errors;
        }
    }
}
