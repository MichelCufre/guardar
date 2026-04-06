using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;
using WIS.Domain.Picking.Dtos;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class CantidadPedidaLpnEspecificoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly DetallePedidoLpnEspecifico _lpnEspecifico;

        public CantidadPedidaLpnEspecificoValidationRule(IUnitOfWork uow, DetallePedidoLpnEspecifico lpnEspecifico)
        {
            this._uow = uow;
            this._lpnEspecifico = lpnEspecifico;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var lpn = _uow.ManejoLpnRepository.GetLpnByIdExternoTipo(_lpnEspecifico.TipoLpn, _lpnEspecifico.IdExternoLpn);

            if (lpn == null)
                errors.Add(new ValidationError("PRE100_msg_error_LpnNoExiste"));
            else
            {
                var qtMaxima = lpn.Detalles
                    .Where(d => d.CodigoProducto == _lpnEspecifico.Producto
                        && (_lpnEspecifico.Identificador != ManejoIdentificadorDb.IdentificadorAuto ? d.Lote == _lpnEspecifico.Identificador : true))
                    .Sum(d => (d.Cantidad - (d.CantidadReserva ?? 0)));

                if (_lpnEspecifico.Cantidad > qtMaxima)
                    errors.Add(new ValidationError("PRE100_msg_error_DetallePedidoLpnCantidadMayorDisponible"));

                if (_lpnEspecifico.Update)
                {
                    var detallePedidoLpn = _uow.ManejoLpnRepository.GetDetallePedidoLpn(_lpnEspecifico.Pedido, _lpnEspecifico.Cliente, _lpnEspecifico.Empresa, _lpnEspecifico.Producto,
                        _lpnEspecifico.Faixa, _lpnEspecifico.Identificador, _lpnEspecifico.IdEspecificaIdentificador, _lpnEspecifico.TipoLpn, _lpnEspecifico.IdExternoLpn);

                    var qtNoDisponible = (detallePedidoLpn.CantidadLiberada ?? 0) + (detallePedidoLpn.CantidadAnulada ?? 0);
                    if (_lpnEspecifico.Cantidad < qtNoDisponible)
                        errors.Add(new ValidationError("PRE100_msg_error_DetallePedidoLpnCantidadMenorSaldo"));

                    var detsPedidoLpnAtributos = _uow.ManejoLpnRepository.GetDetallesPedidoLpnAtributo(_lpnEspecifico.Pedido, _lpnEspecifico.Cliente, _lpnEspecifico.Empresa, _lpnEspecifico.Producto,
                        _lpnEspecifico.Faixa, _lpnEspecifico.Identificador, _lpnEspecifico.IdEspecificaIdentificador, _lpnEspecifico.TipoLpn, _lpnEspecifico.IdExternoLpn);

                    if (detsPedidoLpnAtributos != null && detsPedidoLpnAtributos.Count > 0)
                    {
                        var qtUtilizada = detsPedidoLpnAtributos.Sum(d => (d.CantidadPedida));
                        if (_lpnEspecifico.Cantidad < qtUtilizada)
                            errors.Add(new ValidationError("PRE100_msg_error_DetallePedidoLpnCantidadMenorUtilizada"));
                    }
                }
                else if (_lpnEspecifico.ManejoIdentificador == ManejoIdentificador.Serie && _lpnEspecifico.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                {
                    var detallePedido = _uow.PedidoRepository.GetDetallePedido(_lpnEspecifico.Pedido, _lpnEspecifico.Empresa, _lpnEspecifico.Cliente, _lpnEspecifico.Producto, _lpnEspecifico.Identificador, _lpnEspecifico.Faixa, _lpnEspecifico.IdEspecificaIdentificador);

                    if (detallePedido != null)
                        errors.Add(new ValidationError("PRE100_msg_Error_DetalleProductoSerieExistente"));
                }
            }

            return errors;
        }
    }
}
