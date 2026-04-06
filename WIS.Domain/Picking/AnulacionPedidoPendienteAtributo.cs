using System;
using System.Linq;
using WIS.Domain.DataModel;

namespace WIS.Domain.Picking
{
    public class AnulacionPedidoPendienteAtributo
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Pedido _pedido;
        protected readonly DetallePedidoAtributo _detallePedidoAtributo;
        protected readonly int _usuario;
        protected readonly string _aplicacion;
        protected readonly string _motivo;

        public AnulacionPedidoPendienteAtributo(IUnitOfWork uow, Pedido pedido, DetallePedidoAtributo detallePedidoAtributo, string motivo, int usuario, string aplicacion)
        {
            this._uow = uow;
            this._pedido = pedido;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._detallePedidoAtributo = detallePedidoAtributo;
            this._motivo = string.IsNullOrEmpty(motivo) ? "Anulación de Detalle de Pedido Atributo" : motivo;
        }

        public virtual PedidoAnulado Anular()
        {
            var cantAnular = _detallePedidoAtributo.GetSaldo();

            var pedidoAnulado = CrearPedidoAnulado(cantAnular);

            var detalle = _pedido.Lineas.FirstOrDefault(d => d.Producto == _detallePedidoAtributo.Producto && d.Identificador == _detallePedidoAtributo.Identificador && d.Faixa == _detallePedidoAtributo.Faixa && d.EspecificaIdentificador == (_detallePedidoAtributo.IdEspecificaIdentificador == "S"));

            detalle.Anular(cantAnular);
            detalle.FechaModificacion = DateTime.Now;
            detalle.Transaccion = this._uow.GetTransactionNumber();

            this._uow.PedidoRepository.UpdateDetallePedido(detalle);

            _detallePedidoAtributo.AnularTotal();
            _detallePedidoAtributo.FechaModificacion = DateTime.Now;
            _detallePedidoAtributo.Transaccion = this._uow.GetTransactionNumber();

            _uow.ManejoLpnRepository.UpdateDetallePedidoAtributo(_detallePedidoAtributo);
            _uow.PedidoRepository.AddPedidoAnuladoLpn(AnulacionPedidoPendiente.CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, _detallePedidoAtributo, cantAnular));

            return pedidoAnulado;
        }

        public virtual PedidoAnulado Anular(decimal cantidad)
        {
            var pedidoAnulado = CrearPedidoAnulado(cantidad);

            var detalle = _pedido.Lineas.FirstOrDefault(d => d.Producto == _detallePedidoAtributo.Producto && d.Identificador == _detallePedidoAtributo.Identificador && d.Faixa == _detallePedidoAtributo.Faixa && d.EspecificaIdentificador == (_detallePedidoAtributo.IdEspecificaIdentificador == "S"));

            detalle.Anular(cantidad);
            detalle.FechaModificacion = DateTime.Now;
            detalle.Transaccion = _uow.GetTransactionNumber();

            _uow.PedidoRepository.UpdateDetallePedido(detalle);

            _detallePedidoAtributo.Anular(cantidad);
            _detallePedidoAtributo.FechaModificacion = DateTime.Now;
            _detallePedidoAtributo.Transaccion = _uow.GetTransactionNumber();

            _uow.ManejoLpnRepository.UpdateDetallePedidoAtributo(_detallePedidoAtributo);
            _uow.PedidoRepository.AddPedidoAnuladoLpn(AnulacionPedidoPendiente.CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, _detallePedidoAtributo, cantidad));

            return pedidoAnulado;
        }

        public virtual PedidoAnulado CrearPedidoAnulado(decimal cantidad)
        {
            var registro = new PedidoAnulado
            {
                Pedido = this._pedido.Id,
                Cliente = this._pedido.Cliente,
                Empresa = this._pedido.Empresa,
                Producto = this._detallePedidoAtributo.Producto,
                Embalaje = this._detallePedidoAtributo.Faixa,
                Identificador = this._detallePedidoAtributo.Identificador,
                EspecificaIdentificador = this._detallePedidoAtributo.IdEspecificaIdentificador == "S",
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
