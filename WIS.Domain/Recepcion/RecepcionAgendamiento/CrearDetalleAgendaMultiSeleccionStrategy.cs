using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Recepcion.Enums;
using WIS.Exceptions;
using WIS.TrafficOfficer;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class CrearDetalleAgendaMultiSeleccionStrategy : ICrearDetallesAgendaStrategy
    {
        protected IUnitOfWork _uow;
        protected List<int> _keyReferencias;
        protected Agenda _agenda;
        protected ITrafficOfficerService _concurrencyControl;

        public CrearDetalleAgendaMultiSeleccionStrategy(IUnitOfWork uow, ITrafficOfficerService concurrencyControl, Agenda agenda, List<int> keyReferencias)
        {
            _uow = uow;
            _keyReferencias = keyReferencias;
            _agenda = agenda;
            _concurrencyControl = concurrencyControl;
        }

        /// <summary>
        /// + Crea asociación de cabezales Agenda-Referencias
        /// + Actualiza cantidad agendada de las referencias
        /// + Crea detalle de agenda
        /// + Crea asociación de detalles de Agenda - detalles de Referencia
        /// </summary>
        /// <returns> Detalle de Agenda </returns>
        public virtual List<AgendaDetalle> CrearDetallesAgenda()
        {
            var detalles = new List<AgendaDetalle>();
            var nuTransaccion = _uow.GetTransactionNumber();

            foreach (var key in _keyReferencias)
            {
                var referencia = _uow.ReferenciaRecepcionRepository.GetReferenciaConDetalle(key);

                if (referencia == null)
                    throw new EntityNotFoundException("General_Sec0_Error_NoSeEncontroReferencia", new string[] { key.ToString() });

                if (this._concurrencyControl.IsLocked("T_RECEPCION_REFERENCIA", key.ToString()))
                    throw new EntityLockedException("General_Sec0_Error_ReferenciaBloqueda", new string[] { key.ToString() });

                this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA", key.ToString());

                _uow.ReferenciaRecepcionRepository.AsociarReferenciaAgenda(_agenda.Id, key);

                var recepcionTipo = _uow.RecepcionTipoRepository.GetRecepcionTipo(_agenda.TipoRecepcionInterno);

                if (_agenda.CargaDetalleAutomatica)
                {
                    foreach (var detalleReferencia in referencia.Detalles)
                    {
                        var saldoDetalleReferencia = detalleReferencia.GetSaldo();

                        detalleReferencia.NumeroTransaccion = nuTransaccion;

                        if (saldoDetalleReferencia > 0)
                        {
                            var producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(detalleReferencia.IdEmpresa, detalleReferencia.CodigoProducto);

                            // Puede existir el detalle de agenda ya que son varias referencias.
                            var detalleAgenda = detalles.FirstOrDefault(d => d.CodigoProducto == detalleReferencia.CodigoProducto
                                && d.Identificador == detalleReferencia.Identificador
                                && d.Faixa == detalleReferencia.Faixa
                                && d.IdEmpresa == detalleReferencia.IdEmpresa);

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
                                    CantidadAgendada = 0,
                                    CantidadAgendadaOriginal = 0,
                                    CantidadAceptada = 0,
                                    CantidadCrossDocking = 0,
                                    CantidadRecibida = 0,
                                    CantidadRecibidaFicticia = 0,
                                };

                                detalles.Add(detalleAgenda);
                            }

                            detalleAgenda.CantidadAgendada += saldoDetalleReferencia;
                            detalleAgenda.CantidadAgendadaOriginal += saldoDetalleReferencia;
                            detalleAgenda.NumeroTransaccion = nuTransaccion;


                            this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", detalleReferencia.Id.ToString());

                            detalleReferencia.CantidadAgendada += saldoDetalleReferencia;

                            _uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalle(detalleReferencia);

                            _uow.ReferenciaRecepcionRepository.AsociarDetalleReferenciaConDetalleAgenda(detalleAgenda, detalleReferencia, saldoDetalleReferencia, 0);

                        }
                    }
                }
            }

            if (detalles.Count > 0)
                _uow.AgendaRepository.AddAgendaDetalles(detalles);

            return detalles;
        }
    }
}
