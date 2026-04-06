using System;
using System.Linq;
using WIS.Domain.DataModel;

namespace WIS.Domain.Picking
{
    public class AnulacionPedidoPendienteLpnAtributo
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Pedido _pedido;
        protected readonly DetallePedidoLpnAtributo _detallePedidoLpnAtributo;
        protected readonly int _usuario;
        protected readonly string _aplicacion;
        protected readonly string _motivo;

        public AnulacionPedidoPendienteLpnAtributo(IUnitOfWork uow, Pedido pedido, DetallePedidoLpnAtributo detallePedidoLpnAtributo, string motivo, int usuario, string aplicacion)
        {
            this._uow = uow;
            this._pedido = pedido;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._detallePedidoLpnAtributo = detallePedidoLpnAtributo;
            this._motivo = string.IsNullOrEmpty(motivo) ? "Anulación de Detalle de Pedido LPN Atributo" : motivo;
        }

        public virtual PedidoAnulado Anular()
        {
            var cantAnular = _detallePedidoLpnAtributo.GetSaldo();

            var pedidoAnulado = CrearPedidoAnulado(cantAnular);

            var detalle = _pedido.Lineas.FirstOrDefault(d => d.Producto == _detallePedidoLpnAtributo.Producto && d.Identificador == _detallePedidoLpnAtributo.Identificador && d.Faixa == _detallePedidoLpnAtributo.Faixa && d.EspecificaIdentificador == (_detallePedidoLpnAtributo.IdEspecificaIdentificador == "S"));

            detalle.Anular(cantAnular);
            detalle.FechaModificacion = DateTime.Now;
            detalle.Transaccion = _uow.GetTransactionNumber();

            _uow.PedidoRepository.UpdateDetallePedido(detalle);

            var detallePedidoLpn = _uow.ManejoLpnRepository.GetDetallePedidoLpn(_detallePedidoLpnAtributo.Pedido, _detallePedidoLpnAtributo.IdLpnExterno, _detallePedidoLpnAtributo.Tipo, _detallePedidoLpnAtributo.Cliente, _detallePedidoLpnAtributo.Empresa, _detallePedidoLpnAtributo.Producto, _detallePedidoLpnAtributo.Identificador, _detallePedidoLpnAtributo.Faixa);

            detallePedidoLpn.Anular(cantAnular);
            detallePedidoLpn.FechaModificacion = DateTime.Now;
            detallePedidoLpn.Transaccion = _uow.GetTransactionNumber();

            _uow.ManejoLpnRepository.UpdateDetallePedidoLpn(detallePedidoLpn);
            _uow.PedidoRepository.AddPedidoAnuladoLpn(AnulacionPedidoPendiente.CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, detallePedidoLpn, cantAnular));

            _detallePedidoLpnAtributo.AnularTotal();
            _detallePedidoLpnAtributo.FechaModificacion = DateTime.Now;
            _detallePedidoLpnAtributo.Transaccion = this._uow.GetTransactionNumber();

            _uow.ManejoLpnRepository.UpdateDetallePedidoLpnAtributo(_detallePedidoLpnAtributo);
            _uow.PedidoRepository.AddPedidoAnuladoLpn(AnulacionPedidoPendiente.CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, _detallePedidoLpnAtributo, cantAnular));

            return pedidoAnulado;
        }

        public virtual PedidoAnulado Anular(decimal cantidad)
        {
            var pedidoAnulado = CrearPedidoAnulado(cantidad);

            var detalle = _pedido.Lineas.FirstOrDefault(d => d.Producto == _detallePedidoLpnAtributo.Producto && d.Identificador == _detallePedidoLpnAtributo.Identificador && d.Faixa == _detallePedidoLpnAtributo.Faixa && d.EspecificaIdentificador == (_detallePedidoLpnAtributo.IdEspecificaIdentificador == "S"));

            detalle.Anular(cantidad);
            detalle.FechaModificacion = DateTime.Now;
            detalle.Transaccion = this._uow.GetTransactionNumber();

            this._uow.PedidoRepository.UpdateDetallePedido(detalle);

            var detallePedidoLpn = _uow.ManejoLpnRepository.GetDetallePedidoLpn(_detallePedidoLpnAtributo.Pedido, _detallePedidoLpnAtributo.IdLpnExterno, _detallePedidoLpnAtributo.Tipo, _detallePedidoLpnAtributo.Cliente, _detallePedidoLpnAtributo.Empresa, _detallePedidoLpnAtributo.Producto, _detallePedidoLpnAtributo.Identificador, _detallePedidoLpnAtributo.Faixa);

            detallePedidoLpn.Anular(cantidad);
            detallePedidoLpn.FechaModificacion = DateTime.Now;
            detallePedidoLpn.Transaccion = this._uow.GetTransactionNumber();
            
            _uow.ManejoLpnRepository.UpdateDetallePedidoLpn(detallePedidoLpn);
            _uow.PedidoRepository.AddPedidoAnuladoLpn(AnulacionPedidoPendiente.CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, detallePedidoLpn, cantidad));

            _detallePedidoLpnAtributo.Anular(cantidad);
            _detallePedidoLpnAtributo.FechaModificacion = DateTime.Now;
            _detallePedidoLpnAtributo.Transaccion = this._uow.GetTransactionNumber();

            _uow.ManejoLpnRepository.UpdateDetallePedidoLpnAtributo(_detallePedidoLpnAtributo);
            _uow.PedidoRepository.AddPedidoAnuladoLpn(AnulacionPedidoPendiente.CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, _detallePedidoLpnAtributo, cantidad));
            
            return pedidoAnulado;
        }

        public virtual PedidoAnulado CrearPedidoAnulado(decimal cantidad)
        {
            var registro = new PedidoAnulado
            {
                Pedido = this._pedido.Id,
                Cliente = this._pedido.Cliente,
                Empresa = this._pedido.Empresa,
                Producto = this._detallePedidoLpnAtributo.Producto,
                Embalaje = this._detallePedidoLpnAtributo.Faixa,
                Identificador = this._detallePedidoLpnAtributo.Identificador,
                EspecificaIdentificador = this._detallePedidoLpnAtributo.IdEspecificaIdentificador == "S",
                CantidadAnulada = cantidad,
                Motivo = this._motivo,
                Funcionario = this._usuario,
                FechaInsercion = DateTime.Now,
                Aplicacion = this._aplicacion,
            };

            if (this._pedido.PuedeAnularse())
                registro.PrepararInterfaz();

            this._uow.PedidoRepository.AddPedidoAnulado(registro);

            return registro;
        }
    }
}
