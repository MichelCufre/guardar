using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class CantidadPedidaLpnAtributoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly DetallePedidoLpnEspecifico _datos;

        public CantidadPedidaLpnAtributoValidationRule(IUnitOfWork uow, DetallePedidoLpnEspecifico datos)
        {
            this._uow = uow;
            this._datos = datos;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var detallePedidoLpn = _uow.ManejoLpnRepository.GetDetallePedidoLpn(_datos.Pedido, _datos.Cliente, _datos.Empresa, _datos.Producto, _datos.Faixa,
                _datos.Identificador, _datos.IdEspecificaIdentificador, _datos.TipoLpn, _datos.IdExternoLpn);

            if (detallePedidoLpn == null)
                errors.Add(new ValidationError("PRE100_msg_Error_DetallePedidoLpnExistente"));
            else
            {
                decimal qtMaxima = 0;
                var detsPedidoLpnAtributos = _uow.ManejoLpnRepository.GetDetallesPedidoLpnAtributo(_datos.Pedido, _datos.Cliente, _datos.Empresa, _datos.Producto, _datos.Faixa,
                    _datos.Identificador, _datos.IdEspecificaIdentificador, _datos.TipoLpn, _datos.IdExternoLpn);

                if (_datos.Update && _datos.IdConfiguracion != null)
                {
                    var det = detsPedidoLpnAtributos.FirstOrDefault(d => d.IdConfiguracion == _datos.IdConfiguracion.Value);

                    if (det == null)
                        errors.Add(new ValidationError("PRE100_msg_Error_DetallePedidoLpnAtributoNoExiste"));
                    else
                    {
                        var saldo = (det.CantidadPedida - (det.CantidadLiberada ?? 0) - (det.CantidadAnulada ?? 0));
                        if (_datos.Cantidad < saldo)
                            errors.Add(new ValidationError("PRE100_msg_error_DetallePedidoLpnCantidadMenorSaldo"));
                    }

                    qtMaxima = (detallePedidoLpn.CantidadPedida ?? 0) - (detsPedidoLpnAtributos
                        .Where(d => d.IdConfiguracion != _datos.IdConfiguracion.Value)
                        .Sum(d => (d.CantidadPedida)));
                }
                else
                    qtMaxima = (detallePedidoLpn.CantidadPedida ?? 0) - (detsPedidoLpnAtributos.Sum(d => (d.CantidadPedida)));

                if (qtMaxima <= 0)
                    errors.Add(new ValidationError("PRE100_msg_error_SinSaldoDisponibleNuevoGrupoDeAtributos"));
                else if (_datos.Cantidad > qtMaxima)
                    errors.Add(new ValidationError("PRE100_msg_error_CantidadMayorDisponible", new List<string>() { qtMaxima.ToString() }));
            }

            return errors;
        }
    }
}
