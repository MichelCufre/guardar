using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Recepcion.Enums;
using WIS.Exceptions;
using WIS.TrafficOfficer;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class CrearDetalleAgendaMonoSeleccionStrategy : ICrearDetallesAgendaStrategy
    {
        protected IUnitOfWork _uow;
        protected int _keyReferencia;
        protected Agenda _agenda;
        protected ITrafficOfficerService _concurrencyControl;

        public CrearDetalleAgendaMonoSeleccionStrategy(IUnitOfWork uow, ITrafficOfficerService concurrencyControl, Agenda agenda, int keyReferencia)
        {
            _uow = uow;
            _keyReferencia = keyReferencia;
            _agenda = agenda;
            _concurrencyControl = concurrencyControl;
        }

        /// <summary>
        /// + Crea asociación de cabezales Agenda-Referencia
        /// + Actualiza cantidad agendada de la referencia
        /// + Crea detalle de agenda
        /// + Crea asociación de detalle de Agenda- detalle de Referencia
        /// </summary>
        /// <returns> Detalle de Agenda </returns>
        public virtual List<AgendaDetalle> CrearDetallesAgenda()
        {
            var referencia = _uow.ReferenciaRecepcionRepository.GetReferenciaConDetalle(_keyReferencia);

            if (referencia == null)
                throw new EntityNotFoundException("General_Sec0_Error_NoSeEncontroReferencia", new string[] { _keyReferencia.ToString() });

            if (this._concurrencyControl.IsLocked("T_RECEPCION_REFERENCIA", _keyReferencia.ToString()))
                throw new EntityLockedException("General_Sec0_Error_ReferenciaBloqueda", new string[] { _keyReferencia.ToString() });

            var nuTransaccion = _uow.GetTransactionNumber();

            this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA", _keyReferencia.ToString());

            _uow.ReferenciaRecepcionRepository.AsociarReferenciaAgenda(_agenda.Id, _keyReferencia);

            var recepcionTipo = _uow.RecepcionTipoRepository.GetRecepcionTipo(_agenda.TipoRecepcionInterno);

            var detalles = new List<AgendaDetalle>();

            if (_agenda.CargaDetalleAutomatica)
            {
                foreach (var detalleReferencia in referencia.Detalles)
                {
                    var saldoDetalleReferencia = detalleReferencia.GetSaldo();

                    detalleReferencia.NumeroTransaccion = nuTransaccion;

                    if (saldoDetalleReferencia > 0)
                    {
                        var producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(detalleReferencia.IdEmpresa, detalleReferencia.CodigoProducto);

                        var detalleAgenda = detalles.FirstOrDefault(x => x.IdEmpresa == _agenda.IdEmpresa && x.CodigoProducto == detalleReferencia.CodigoProducto && x.Faixa == detalleReferencia.Faixa && x.Identificador == detalleReferencia.Identificador);

                        if (detalleAgenda == null)
                        {
                            detalleAgenda = new AgendaDetalle()
                            {
                                IdAgenda = _agenda.Id,
                                IdEmpresa = _agenda.IdEmpresa,
                                CodigoProducto = detalleReferencia.CodigoProducto,
                                Faixa = detalleReferencia.Faixa,
                                Identificador = detalleReferencia.Identificador,
                                Estado = EstadoAgendaDetalle.Abierta,
                                Vencimiento = detalleReferencia.FechaVencimiento ?? producto.GetFechaVencimiento(),
                                CantidadAgendada = saldoDetalleReferencia,
                                CantidadAgendadaOriginal = saldoDetalleReferencia,
                                CantidadAceptada = 0,
                                CantidadCrossDocking = 0,
                                CantidadRecibida = 0,
                                CantidadRecibidaFicticia = 0,
                            };

                            detalles.Add(detalleAgenda);
                        }
                        else
                        {
                            detalleAgenda.CantidadAgendada += saldoDetalleReferencia;
                            detalleAgenda.CantidadAgendadaOriginal += saldoDetalleReferencia;
                        }

                        detalleAgenda.NumeroTransaccion = nuTransaccion;

                        this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", detalleReferencia.Id.ToString());

                        detalleReferencia.CantidadAgendada += saldoDetalleReferencia;
                        _uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalle(detalleReferencia);

                        _uow.ReferenciaRecepcionRepository.AsociarDetalleReferenciaConDetalleAgenda(detalleAgenda, detalleReferencia, saldoDetalleReferencia, 0);
                    }
                }

                if (detalles.Count > 0)
                    _uow.AgendaRepository.AddAgendaDetalles(detalles);
            }

            return detalles;
        }
    }
}
