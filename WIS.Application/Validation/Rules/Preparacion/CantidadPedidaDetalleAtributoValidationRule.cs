using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;
using WIS.Domain.Picking.Dtos;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class CantidadPedidaDetalleAtributoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly DetallePedidoLpnEspecifico _datos;

        public CantidadPedidaDetalleAtributoValidationRule(IUnitOfWork uow, DetallePedidoLpnEspecifico datos)
        {
            this._uow = uow;
            this._datos = datos;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_datos.Update && _datos.IdConfiguracion != null)
            {
                var detAtributo = _uow.ManejoLpnRepository.GetDetallePedidoAtributo(_datos.Pedido, _datos.Cliente, _datos.Empresa, _datos.Producto, _datos.Faixa,
                    _datos.Identificador, _datos.IdEspecificaIdentificador, _datos.IdConfiguracion.Value);

                if (detAtributo == null)
                    errors.Add(new ValidationError("PRE100_msg_Error_DetallePedidoAtributoNoExiste"));
                else
                {
                    var saldoNoDisponible = (detAtributo.CantidadLiberada ?? 0) + (detAtributo.CantidadAnulada ?? 0);
                    if (_datos.Cantidad < saldoNoDisponible)
                        errors.Add(new ValidationError("PRE100_msg_error_DetallePedidoAtributoCantidadMenorSaldo"));
                }
            }
            else if( !_datos.Update && _datos.ManejoIdentificador == ManejoIdentificador.Serie && _datos.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
            {
                var detallePedido = _uow.PedidoRepository.GetDetallePedido(_datos.Pedido, _datos.Empresa, _datos.Cliente, _datos.Producto, _datos.Identificador, _datos.Faixa, _datos.IdEspecificaIdentificador);

                if (detallePedido != null)
                    errors.Add(new ValidationError("PRE100_msg_Error_DetalleProductoSerieExistente"));
            }

            return errors;
        }
    }
}
