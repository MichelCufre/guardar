using NLog;
using System;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Exceptions;
using WIS.TrafficOfficer;

namespace WIS.Domain.Recepcion
{
    public class CancelarAgenda
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWork _uow;
        protected ITrafficOfficerService _concurrencyControl;
        protected string _aplicacion;
        protected int _usuario;
        protected CrossDockingAgenda _crossDockingAgenda;
        protected Agenda _agenda;

        public CancelarAgenda(IUnitOfWork uow, ITrafficOfficerService concurrencyControl, int usuario, string aplicacion, Agenda agenda)
        {
            this._uow = uow;
            this._concurrencyControl = concurrencyControl;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._agenda = agenda;
            this._crossDockingAgenda = new CrossDockingAgenda(uow, agenda);
        }

        public virtual void ProcesarCancelarAgenda(TrafficOfficerTransaction transaction)
        {
            if (!_agenda.PuedeCancelarAgenda(_uow))
                throw new OperationNotAllowedException("REC170_Sec0_Error_AgendaNoCancelable");

            // Contenedores de cambios en orden de actualización
            var cambiosDetalleReferencia = new EntityChanges<ReferenciaRecepcionDetalle>();
            var cambiosAsociaciones = new EntityChanges<DetalleAgendaReferenciaAsociada>();
            var cambiosReferencia = new EntityChanges<ReferenciaRecepcion>();

            LockearAgendaConDetalles(transaction);

            //Revertir cantidades agendadas en referencias
            CancelarReferenciasAgenda(transaction, cambiosDetalleReferencia, cambiosAsociaciones, cambiosReferencia);


            _agenda.Estado = EstadoAgenda.Cancelada;
            _agenda.NumeroTransaccion = _uow.GetTransactionNumber();

            #region - IMPACTAR CAMBIOS -

            // Persisto modificaciones en agenda
            _uow.AgendaRepository.UpdateAgendaSinDependencias(_agenda);

            // Persisto modificaciones de las referencias
            UpdateReferenciaDetalles(cambiosDetalleReferencia);

            // Persisto las asociaciones de las referencias y los detalles de agenda
            UpdateDetalleAgendaReferenciaAsociada(cambiosAsociaciones);

            // Persisto modificaciones de las referencias cabezales
            UpdateReferencia(cambiosReferencia);

            #endregion

            _crossDockingAgenda.DesatenderPedidosCrossDocking(true);

            DesvincularFacturas(_agenda);

            DesvincularDocumento(_agenda);

            DesvincularLpns(_agenda);
        }

        public virtual void LockearAgendaConDetalles(TrafficOfficerTransaction transaction)
        {
            if (_concurrencyControl.IsLocked("T_AGENDA", _agenda.GetCompositeId(), true))
                throw new ValidationFailedException("REC170_msg_Error_AgendaBloqueada", [_agenda.GetCompositeId()]);

            _concurrencyControl.AddLock("T_AGENDA", _agenda.GetCompositeId(), transaction, true);

            foreach (var detalle in _agenda.Detalles)
            {
                _concurrencyControl.AddLock("T_DET_AGENDA", detalle.GetCompositeId(), transaction, true);
            }
        }

        public virtual void CancelarReferenciasAgenda(TrafficOfficerTransaction transaction, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcion> cambiosReferencia)
        {
            logger.Debug($"CANCELAR_AGENDA: {_agenda.Id} - Cancelar referencias asociadas");

            if (_uow.ReferenciaRecepcionRepository.AnyAsociacionesAgendaConDetallesReferencias(_agenda.Id))
            {
                // Busco las asociaciones de la agenda para retornar saldo a los detalles de referencia
                var asociaciones = _uow.ReferenciaRecepcionRepository.GetAllDetalleAgendaReferenciaAsociadaActivas(_agenda);

                foreach (var asociacion in asociaciones)
                {
                    //Lockeo detalle
                    this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", asociacion.DetalleReferencia.GetCompositeId(), transaction);

                    // Retorno consumido en la asociación al detalle de referencia
                    asociacion.DetalleReferencia.CantidadAgendada -= asociacion.CantidadAgendada;
                    UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);

                    //Eliminar relacion referencia agenda
                    cambiosAsociaciones.DeletedRecords.Add(asociacion);

                    // Comprobar saldos de las referencias para cambiar estado a abierta si hay saldo
                    if (asociacion.DetalleReferencia.ReferenciaRecepcion.Estado == EstadoReferenciaRecepcionDb.Finalizada)
                    {
                        // Si llega a este punto se esta retornando saldo a las referencias entoces cambio estado a abierto
                        asociacion.DetalleReferencia.ReferenciaRecepcion.Estado = EstadoReferenciaRecepcionDb.Abierta;
                        UpdateRecordReferencia(cambiosReferencia, asociacion.DetalleReferencia.ReferenciaRecepcion);
                    }
                }
            }
        }

        public virtual void UpdateRecordDetalleReferencia(EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia, ReferenciaRecepcionDetalle detalleReferencia)
        {
            var record = cambiosDetalleReferencia.UpdatedRecords.FirstOrDefault(d => d.Id == detalleReferencia.Id);

            if (record != null)
                cambiosDetalleReferencia.UpdatedRecords.Remove(record);

            cambiosDetalleReferencia.UpdatedRecords.Add(detalleReferencia);

        }
        public virtual void UpdateRecordReferencia(EntityChanges<ReferenciaRecepcion> cambiosReferencia, ReferenciaRecepcion referencia)
        {
            var record = cambiosReferencia.UpdatedRecords.FirstOrDefault(d => d.Id == referencia.Id);

            if (record != null)
                cambiosReferencia.UpdatedRecords.Remove(record);

            cambiosReferencia.UpdatedRecords.Add(referencia);

        }

        public virtual void UpdateReferenciaDetalles(EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            cambiosDetalleReferencia.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosDetalleReferencia.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            foreach (var dr in cambiosDetalleReferencia.DeletedRecords)
            {
                dr.NumeroTransaccion = nuTransaccion;
                dr.NumeroTransaccionDelete = nuTransaccion;
                dr.FechaModificacion = DateTime.Now;

                _uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalle(dr);
            }

            _uow.SaveChanges();
            _uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalles(cambiosDetalleReferencia);
        }

        public virtual void UpdateReferencia(EntityChanges<ReferenciaRecepcion> cambiosReferencia)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            cambiosReferencia.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosReferencia.DeletedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosReferencia.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            _uow.ReferenciaRecepcionRepository.UpdateReferencia(cambiosReferencia);
        }

        public virtual void UpdateDetalleAgendaReferenciaAsociada(EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            cambiosAsociaciones.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosAsociaciones.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            foreach (var dr in cambiosAsociaciones.DeletedRecords)
            {
                dr.NumeroTransaccion = nuTransaccion;
                dr.NumeroTransaccionDelete = nuTransaccion;
                _uow.ReferenciaRecepcionRepository.UpdateDetalleAgendaReferenciaAsociada(dr);
            }

            _uow.SaveChanges();
            _uow.ReferenciaRecepcionRepository.UpdateDetalleAgendaReferenciaAsociada(cambiosAsociaciones);
        }

        public virtual void DesvincularDocumento(Agenda agenda)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            if (_uow.EmpresaRepository.IsEmpresaDocumental(agenda.IdEmpresa))
            {
                _uow.DocumentoRepository.DesvincularAgenda(agenda, nuTransaccion);
            }
        }

        public virtual void DesvincularFacturas(Agenda agenda)
        {
            _uow.AgendaRepository.DesvincularFacturas(agenda, _uow.GetTransactionNumber());
        }

        public virtual void DesvincularLpns(Agenda agenda)
        {
            _uow.AgendaRepository.DesvincularLpns(agenda.Id, _uow.GetTransactionNumber());
        }
    }
}
