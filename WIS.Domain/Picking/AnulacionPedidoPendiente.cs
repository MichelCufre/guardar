using System;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Bulks;

namespace WIS.Domain.Picking
{
    public class AnulacionPedidoPendiente
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Pedido _pedido;
        protected readonly DetallePedido _detalle;
        protected readonly int _usuario;
        protected readonly string _aplicacion;
        protected readonly string _motivo;

        public AnulacionPedidoPendiente(IUnitOfWork uow, Pedido pedido, DetallePedido detalle, string motivo, int usuario, string aplicacion)
        {
            this._uow = uow;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._pedido = pedido;
            this._detalle = detalle;
            this._motivo = string.IsNullOrEmpty(motivo) ? "Anulación de Detalle de Pedido" : motivo;
        }

        public virtual PedidoAnulado Anular()
        {
            var context = new PedidoAnuladoLpnBulkOperationContext();

            var pedidoAnulado = this.CrearPedidoAnulado(this._detalle.GetSaldo());

            this._detalle.AnularTotal();
            this._detalle.FechaModificacion = DateTime.Now;
            this._detalle.Transaccion = this._uow.GetTransactionNumber();

            this._uow.PedidoRepository.UpdateDetallePedido(this._detalle);

            this._uow.SaveChanges();

            var detallesPedidoLpn = _uow.ManejoLpnRepository.GetDetallesPedidoLpn(this._detalle.Id, this._detalle.Cliente, this._detalle.Empresa, this._detalle.Producto, this._detalle.Identificador, this._detalle.Faixa);
            foreach (var detallePedidoLpn in detallesPedidoLpn)
            {
                //Respetar orden
                var cantAnularLog = detallePedidoLpn.GetSaldo();

                detallePedidoLpn.CantidadAnulada = (detallePedidoLpn.CantidadPedida ?? 0) - (detallePedidoLpn.CantidadLiberada ?? 0);
                detallePedidoLpn.Transaccion = this._uow.GetTransactionNumber();
                detallePedidoLpn.FechaModificacion = DateTime.Now;

                context.UpdateDetallePedidoLpn.Add(detallePedidoLpn);
                context.NewLogPedidoAnuladoLpn.Add(CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, detallePedidoLpn, cantAnularLog));
            }

            var detallesPedidoLpnAtributo = _uow.ManejoLpnRepository.GetDetallesPedidoAtributoLpn(this._detalle.Id, this._detalle.Cliente, this._detalle.Empresa, this._detalle.Producto, this._detalle.Identificador, this._detalle.Faixa);
            foreach (var detallePedidoLpnAtributo in detallesPedidoLpnAtributo)
            {
                //Respetar orden
                var cantAnularLog = detallePedidoLpnAtributo.GetSaldo();

                detallePedidoLpnAtributo.CantidadAnulada = detallePedidoLpnAtributo.CantidadPedida - (detallePedidoLpnAtributo.CantidadLiberada ?? 0);
                detallePedidoLpnAtributo.Transaccion = this._uow.GetTransactionNumber();
                detallePedidoLpnAtributo.FechaModificacion = DateTime.Now;

                context.UpdateDetallePedidoLpnAtributo.Add(detallePedidoLpnAtributo);
                context.NewLogPedidoAnuladoLpn.Add(CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, detallePedidoLpnAtributo, cantAnularLog));

            }

            var detallesPedidoAtributo = _uow.ManejoLpnRepository.GetDetallesPedidoAtributo(this._detalle.Id, this._detalle.Cliente, this._detalle.Empresa, this._detalle.Producto, this._detalle.Identificador, this._detalle.Faixa);
            foreach (var detallePedidoAtributo in detallesPedidoAtributo)
            {
                //Respetar orden
                var cantAnularLog = detallePedidoAtributo.GetSaldo();

                detallePedidoAtributo.CantidadAnulada = detallePedidoAtributo.CantidadPedida - (detallePedidoAtributo.CantidadLiberada ?? 0);
                detallePedidoAtributo.Transaccion = this._uow.GetTransactionNumber();
                detallePedidoAtributo.FechaModificacion = DateTime.Now;

                context.UpdateDetallePedidoAtributo.Add(detallePedidoAtributo);
                context.NewLogPedidoAnuladoLpn.Add(CrearPedidoAnuladoLpn(_uow, pedidoAnulado.Id, detallePedidoAtributo, cantAnularLog));
            }

            _uow.ManejoLpnRepository.AnulacionPedidoLpn(context);

            return pedidoAnulado;
        }

        public virtual PedidoAnulado Anular(decimal cantidad)
        {
            var pedidoAnulado = this.CrearPedidoAnulado(cantidad);

            this._detalle.Anular(cantidad);
            this._detalle.FechaModificacion = DateTime.Now;
            this._detalle.Transaccion = this._uow.GetTransactionNumber();

            this._uow.PedidoRepository.UpdateDetallePedido(this._detalle);

            return pedidoAnulado;
        }

        public virtual PedidoAnulado CrearPedidoAnulado(decimal cantidad)
        {
            var registro = new PedidoAnulado
            {
                Pedido = this._pedido.Id,
                Cliente = this._pedido.Cliente,
                Empresa = this._pedido.Empresa,
                Producto = this._detalle.Producto,
                Embalaje = this._detalle.Faixa,
                Identificador = this._detalle.Identificador,
                EspecificaIdentificador = this._detalle.EspecificaIdentificador,
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

        public static PedidoAnuladoLpn CrearPedidoAnuladoLpn(IUnitOfWork uow, long logId, DetallePedidoLpn detalle, decimal cantAnular)
        {
            return new PedidoAnuladoLpn
            {
                Id = uow.PedidoRepository.GetNextIdLogPedidoAnuladoLpn(),
                IdLogPedidoAnulado = logId,
                TipoOperacion = TipoAnulacionLpn.PedidoLpn,
                IdExternoLpn = detalle.IdLpnExterno,
                TipoLpn = detalle.Tipo,
                CantidadAnulada = cantAnular,
                FechaInsercion = DateTime.Now,
            };
        }
        public static PedidoAnuladoLpn CrearPedidoAnuladoLpn(IUnitOfWork uow, long logId, DetallePedidoLpnAtributo detalle, decimal cantAnular)
        {
            return new PedidoAnuladoLpn
            {
                Id = uow.PedidoRepository.GetNextIdLogPedidoAnuladoLpn(),
                IdLogPedidoAnulado = logId,
                TipoOperacion = TipoAnulacionLpn.PedidoLpnAtributo,
                IdExternoLpn = detalle.IdLpnExterno,
                TipoLpn = detalle.Tipo,
                IdConfiguracion = detalle.IdConfiguracion,
                CantidadAnulada = cantAnular,
                FechaInsercion = DateTime.Now,
            };
        }
        public static PedidoAnuladoLpn CrearPedidoAnuladoLpn(IUnitOfWork uow, long logId, DetallePedidoAtributo detalle, decimal cantAnular)
        {
            return new PedidoAnuladoLpn
            {
                Id = uow.PedidoRepository.GetNextIdLogPedidoAnuladoLpn(),
                IdLogPedidoAnulado = logId,
                TipoOperacion = TipoAnulacionLpn.PedidoAtributo,
                IdConfiguracion = detalle.IdConfiguracion,
                CantidadAnulada = cantAnular,
                FechaInsercion = DateTime.Now,
            };
        }
    }
}
