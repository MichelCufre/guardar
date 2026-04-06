using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class NoExistePedidoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _pedido;
        protected readonly string _cliente;
        protected readonly string _empresa;

        public NoExistePedidoValidationRule(IUnitOfWork uow, string pedido, string cliente, string empresa)
        {
            this._uow = uow;
            this._pedido = pedido;
            this._cliente = cliente;
            this._empresa = empresa;
        }
        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            int empresaId = int.Parse(this._empresa);

            if (this._uow.PedidoRepository.AnyPedido(empresaId, this._cliente, this._pedido))
                errors.Add(new ValidationError("WPRE100_Sec0_Error_Er001_PedidoExistente"));

            return errors;
        }
    }
}
