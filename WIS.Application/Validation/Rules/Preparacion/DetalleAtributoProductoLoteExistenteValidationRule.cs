using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Exceptions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class DetalleAtributoProductoLoteExistenteValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly DetallePedidoLpnEspecifico _datos;
        protected readonly string _producto;
        protected readonly string _identificador;

        public DetalleAtributoProductoLoteExistenteValidationRule(IUnitOfWork uow, DetallePedidoLpnEspecifico datos, string producto, string identificador)
        {
            this._uow = uow;
            this._datos = datos;
            this._producto = producto;
            this._identificador = identificador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_datos.Update)
            {
                var producto = _uow.ProductoRepository.GetProducto(_datos.Empresa, _producto);

                var identificador = producto.IsIdentifiedByProducto() ? ManejoIdentificadorDb.IdentificadorProducto :
                (string.IsNullOrEmpty(_identificador) ? ManejoIdentificadorDb.IdentificadorAuto : _identificador);

                var idEspecificaIdentificador = (!string.IsNullOrEmpty(identificador) && identificador != ManejoIdentificadorDb.IdentificadorAuto) ? "S" : "N";

                if (_uow.ManejoLpnRepository.AnyProductoDetPedidoAtributo(_datos.Pedido, _datos.Cliente, _datos.Empresa, _producto, _datos.Faixa, identificador, idEspecificaIdentificador))
                    errors.Add(new ValidationError("PRE100_msg_Error_AsociacionAtributoExistente", new List<string> { _producto, identificador }));
            }
            return errors;
        }
    }
}
