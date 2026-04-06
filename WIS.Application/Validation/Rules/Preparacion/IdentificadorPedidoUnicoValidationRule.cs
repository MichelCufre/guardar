using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class IdentificadorPedidoUnicoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _pedido;
        protected readonly string _cliente;
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly string _identificador;

        public IdentificadorPedidoUnicoValidationRule(IUnitOfWork uow, int empresa, string cliente, string pedido, string producto, string identificador)
        {
            this._uow = uow;
            this._pedido = pedido;
            this._cliente = cliente;
            this._empresa = empresa;
            this._producto = producto;
            this._identificador = identificador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(this._empresa, this._producto);

            if (producto.IsIdentifiedByLote() || producto.IsIdentifiedBySerie())
            {
                if (this._uow.PedidoRepository.AnyPedidoIdentificador(this._empresa, this._cliente, this._pedido, this._producto, this._identificador))
                    errors.Add(new ValidationError("PRE100DetallePedido_grid1_error_ProductoYaExistePedido"));
            }

            return errors;
        }
    }
}
