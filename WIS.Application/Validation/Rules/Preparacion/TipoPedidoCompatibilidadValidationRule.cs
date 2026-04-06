using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class TipoPedidoCompatibilidadValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _tipoExpedicion;
        protected readonly string _tipoPedido;

        public TipoPedidoCompatibilidadValidationRule(IUnitOfWork uow, string tipoPedido, string tipoExpedicion)
        {
            this._uow = uow;
            this._tipoPedido = tipoPedido;
            this._tipoExpedicion = tipoExpedicion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (!this._uow.PedidoRepository.IsTipoPedidoCompatibleTipoExpedicion(this._tipoPedido, this._tipoExpedicion))
                errors.Add(new ValidationError("WPRE100_Sec0_Error_Er002_PedidoIncompatibleExpedicion"));

            return errors;
        }
    }
}
