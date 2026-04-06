using System;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.General.API.Bulks;

namespace WIS.Domain.Picking
{
    public class AnulacionPedidoPendienteLpn
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Pedido _pedido;
        protected readonly DetallePedidoLpn _detallePedidoLpn;
        protected readonly int _usuario;
        protected readonly string _aplicacion;
        protected readonly string _motivo;

        public AnulacionPedidoPendienteLpn(IUnitOfWork uow, Pedido pedido, DetallePedidoLpn detallePedidoLpn, string motivo, int usuario, string aplicacion)
        {
            this._uow = uow;
            this._pedido = pedido;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._detallePedidoLpn = detallePedidoLpn;
            this._motivo = string.IsNullOrEmpty(motivo) ? "Anulación de Detalle de Pedido LPN" : motivo;
        }

        public virtual PedidoAnulado Anular()
        {
            var context = new PedidoAnuladoLpnBulkOperationContext();

            var cantAnular = _detallePedidoLpn.GetSaldo();
            var pedidoAnulado = CrearPedidoAnulado(cantAnular);

            var detalle = _pedido.Lineas.FirstOrDefault(d => d.Producto == _detallePedidoLpn.Producto && d.Identificador == _detallePedidoLpn.Identificador && d.Faixa == _detallePedidoLpn.Faixa && d.EspecificaIdentificador == (_detallePedidoLpn.IdEspecificaIdentificador == "S"));

            detalle.Anular(cantAnular);
            detalle.FechaModificacion = DateTime.Now;
            detalle.Transaccion = _uow.GetTransactionNumber();

            _uow.PedidoRepository.UpdateDetallePedido(detalle);

            _detallePedidoLpn.AnularTotal();
            _detallePedidoLpn.FechaModificacion = DateTime.Now;
            _detallePedidoLpn.Transaccion = _uow.GetTransactionNumber();

            _uow.ManejoLpnRepository.UpdateDetallePedidoLpn(_detallePedidoLpn);

            context.NewLogPedidoAnuladoLpn.Add(AnulacionPedidoPendiente.CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, _detallePedidoLpn, cantAnular));

            var detallesPedidoLpnAtributo = _uow.ManejoLpnRepository.GetDetallesPedidoAtributoLpn(_detallePedidoLpn.Pedido, _detallePedidoLpn.Cliente, _detallePedidoLpn.Empresa, _detallePedidoLpn.Producto, _detallePedidoLpn.Identificador, _detallePedidoLpn.Faixa);

            foreach (var detallePedidoLpnAtributo in detallesPedidoLpnAtributo)
            {
                //Respetar orden
                var cantAnularLog = detallePedidoLpnAtributo.GetSaldo();

                detallePedidoLpnAtributo.CantidadAnulada = detallePedidoLpnAtributo.CantidadPedida - (detallePedidoLpnAtributo.CantidadLiberada ?? 0);
                detallePedidoLpnAtributo.Transaccion = this._uow.GetTransactionNumber();
                detallePedidoLpnAtributo.FechaModificacion = DateTime.Now;

                context.UpdateDetallePedidoLpnAtributo.Add(detallePedidoLpnAtributo);
                context.NewLogPedidoAnuladoLpn.Add(AnulacionPedidoPendiente.CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, detallePedidoLpnAtributo, cantAnularLog));
            }

            _uow.ManejoLpnRepository.AnulacionPedidoLpn(context);

            return pedidoAnulado;
        }

        public virtual PedidoAnulado Anular(decimal cantidad)
        {
            var pedidoAnulado = CrearPedidoAnulado(cantidad);

            var detalle = _pedido.Lineas.FirstOrDefault(d => d.Producto == _detallePedidoLpn.Producto && d.Identificador == _detallePedidoLpn.Identificador && d.Faixa == _detallePedidoLpn.Faixa && d.EspecificaIdentificador == (_detallePedidoLpn.IdEspecificaIdentificador == "S"));

            detalle.Anular(cantidad);
            detalle.FechaModificacion = DateTime.Now;
            detalle.Transaccion = _uow.GetTransactionNumber();

            _uow.PedidoRepository.UpdateDetallePedido(detalle);

            _detallePedidoLpn.Anular(cantidad);
            _detallePedidoLpn.FechaModificacion = DateTime.Now;
            _detallePedidoLpn.Transaccion = _uow.GetTransactionNumber();

            _uow.ManejoLpnRepository.UpdateDetallePedidoLpn(_detallePedidoLpn);
            _uow.PedidoRepository.AddPedidoAnuladoLpn(AnulacionPedidoPendiente.CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, _detallePedidoLpn, cantidad));

            return pedidoAnulado;
        }

        public virtual PedidoAnulado CrearPedidoAnulado(decimal cantidad)
        {
            var registro = new PedidoAnulado
            {
                Pedido = this._pedido.Id,
                Cliente = this._pedido.Cliente,
                Empresa = this._pedido.Empresa,
                Producto = this._detallePedidoLpn.Producto,
                Embalaje = this._detallePedidoLpn.Faixa,
                Identificador = this._detallePedidoLpn.Identificador,
                EspecificaIdentificador = this._detallePedidoLpn.IdEspecificaIdentificador == "S",
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
