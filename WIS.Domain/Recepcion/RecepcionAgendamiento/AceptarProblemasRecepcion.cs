using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

using WIS.Domain.DataModel;
using WIS.TrafficOfficer;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class AceptarProblemasRecepcion
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWork _uow;
        protected ITrafficOfficerService _concurrencyControl;
        protected string _aplicacion;
        protected int _usuario;

        public AceptarProblemasRecepcion(IUnitOfWork uow, ITrafficOfficerService concurrencyControl, int usuario, string aplicacion)
        {
            this._uow = uow;
            this._concurrencyControl = concurrencyControl;
            this._usuario = usuario;
            this._aplicacion = aplicacion;

        }

        public virtual void AceptarProblemas(Dictionary<int, int> problemasAgendas)
        {
            EntityChanges<AgendaDetalle> cambiosDetalleAgenda = new EntityChanges<AgendaDetalle>();
            EntityChanges<AgendaDetalleProblema> cambiosProblemas = new EntityChanges<AgendaDetalleProblema>();

            var nuTransaccion = _uow.GetTransactionNumber();

            // Obtengo la lista de agendas afectadas
            var keysAgendas = problemasAgendas.Values.Distinct();
            var agendas = new List<Agenda>();

            // Obtengo las agendas afectadas con sus detalles y problemas correspondientes
            foreach (var idAgenda in keysAgendas)
            {
                agendas.Add(_uow.AgendaRepository.GetAgendaConDetalleProblemas(idAgenda));
            }

            //Recorrro los problemas a aceptar
            foreach (var idProblema in problemasAgendas.Keys)
            {
                // Busco detalle para actualizar
                var detalleAgenda = agendas.FirstOrDefault(a => a.Id == problemasAgendas[idProblema]).GetDetalleAgendaAfectadoProblema(idProblema);

                detalleAgenda.CantidadAceptada = detalleAgenda.CantidadRecibida;
                detalleAgenda.FechaAceptacionRecepcion = DateTime.Now;
                detalleAgenda.IdUsuarioAceptacionRecepcion = _usuario;
                detalleAgenda.FechaModificacion = DateTime.Now;
                detalleAgenda.NumeroTransaccion = nuTransaccion;

                // Aceptar Problema de detalle
                var problema = detalleAgenda.GetProblemaRecepcion(idProblema);

                problema.AceptarProblema(_usuario);
                this.UpdateRecordDetalleAgendaProblema(cambiosProblemas, problema);

                // Compruebo que el detalle no tiene mas problemas y cambio situación
                detalleAgenda.ComprobarCambioEstadoSinDiferencia();
                this.UpdateRecordDetalleAgenda(cambiosDetalleAgenda, detalleAgenda);
            }

            // Comprobar estados de agendas afectadas y marcar sin diferencia si no tiene problemas
            foreach (var agenda in agendas)
            {
                if (agenda.ComprobarYCambiarEstadoAgendaSinDiferencias())
                {
                    agenda.NumeroTransaccion = nuTransaccion;
                    _uow.AgendaRepository.UpdateAgendaSinDependencias(agenda);
                }
            }

            //  Persisto modificaciones en detalles de agenda
            UpdateAgendaDetalles(cambiosDetalleAgenda);

            /// Persisto modificaciones en Problemas
            _uow.AgendaRepository.UpdateAgendaDetalleProblema(cambiosProblemas);
        }

        public virtual void AceptarProblemas(List<int> listaProblemas)
        {
            EntityChanges<AgendaDetalle> cambiosDetalleAgenda = new EntityChanges<AgendaDetalle>();
            EntityChanges<AgendaDetalleProblema> cambiosProblemas = new EntityChanges<AgendaDetalleProblema>();

            var nuTransaccion = _uow.GetTransactionNumber();

            //cargo Dictionary para dejar el resto del codigo como estba antes
            var problemasAgendas = new Dictionary<int, int>();
            foreach (var idProblema in listaProblemas)
            {
                var agendaDetProblema = _uow.AgendaRepository.GetAgendaProblema(idProblema);
                problemasAgendas.Add(agendaDetProblema.Id, agendaDetProblema.NumeroAgenda);
            }

            // Obtengo la lista de agendas afectadas
            var keysAgendas = problemasAgendas.Values.Distinct();

            var agendas = new List<Agenda>();

            // Obtengo las agendas afectadas con sus detalles y problemas correspondientes
            foreach (var idAgenda in keysAgendas)
            {
                agendas.Add(_uow.AgendaRepository.GetAgendaConDetalleProblemas(idAgenda));
            }

            //Recorrro los problemas a aceptar
            foreach (var idProblema in problemasAgendas.Keys)
            {
                // Busco detalle para actualizar
                var detalleAgenda = agendas.FirstOrDefault(a => a.Id == problemasAgendas[idProblema]).GetDetalleAgendaAfectadoProblema(idProblema);

                if (detalleAgenda != null)
                {
                    detalleAgenda.CantidadAceptada = detalleAgenda.CantidadRecibida;
                    detalleAgenda.FechaAceptacionRecepcion = DateTime.Now;
                    detalleAgenda.IdUsuarioAceptacionRecepcion = _usuario;
                    detalleAgenda.FechaModificacion = DateTime.Now;
                    detalleAgenda.NumeroTransaccion = nuTransaccion;

                    // Aceptar Problema de detalle
                    var problema = detalleAgenda.GetProblemaRecepcion(idProblema);
                    logger.Info("Acepto problema: " + problema.Id);

                    problema.AceptarProblema(_usuario);
                    this.UpdateRecordDetalleAgendaProblema(cambiosProblemas, problema);

                    logger.Info("Compruebo que el detalle no tiene mas problemas y cambio situación");
                    // Compruebo que el detalle no tiene mas problemas y cambio situación
                    detalleAgenda.ComprobarCambioEstadoSinDiferencia();
                    logger.Info("Esado de detalle: " + detalleAgenda.Estado.ToString());
                    this.UpdateRecordDetalleAgenda(cambiosDetalleAgenda, detalleAgenda);
                }
                else
                {
                    var problema = _uow.AgendaRepository.GetAgendaProblema(idProblema);

                    if (problema != null)
                    {
                        problema.AceptarProblema(_usuario);
                        UpdateRecordDetalleAgendaProblema(cambiosProblemas, problema);
                    }
                }
            }

            // Comprobar estados de agendas afectadas y marcar sin diferencia si no tiene problemas
            logger.Info("Comprobacion de agendas sin diferencias.");
            foreach (var agenda in agendas)
            {
                logger.Info("Comprobacion agenda " + agenda.Id);

                if (agenda.ComprobarYCambiarEstadoAgendaSinDiferencias())
                {
                    logger.Info("Agenda " + agenda.Id + " sin detalles con difrencias");

                    agenda.NumeroTransaccion = nuTransaccion;

                    _uow.AgendaRepository.UpdateAgendaSinDependencias(agenda);
                }
            }

            //  Persisto modificaciones en detalles de agenda
            UpdateAgendaDetalles(cambiosDetalleAgenda);

            /// Persisto modificaciones en Problemas
            _uow.AgendaRepository.UpdateAgendaDetalleProblema(cambiosProblemas);
        }

        public virtual void UpdateAgendaDetalles(EntityChanges<AgendaDetalle> cambiosDetalleAgenda)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            cambiosDetalleAgenda.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosDetalleAgenda.DeletedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosDetalleAgenda.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            _uow.AgendaRepository.UpdateAgendaDetalles(cambiosDetalleAgenda);
        }

        public virtual void UpdateRecordDetalleAgenda(EntityChanges<AgendaDetalle> cambiosDetalleAgenda, AgendaDetalle detalleAgenda)
        {
            var record = cambiosDetalleAgenda.UpdatedRecords.FirstOrDefault(d => d.CodigoProducto == detalleAgenda.CodigoProducto
                                                                        && d.Faixa == detalleAgenda.Faixa
                                                                        && d.Identificador == detalleAgenda.Identificador
                                                                        && d.IdEmpresa == detalleAgenda.IdEmpresa
                                                                        && d.IdAgenda == detalleAgenda.IdAgenda);

            if (record != null)
                cambiosDetalleAgenda.UpdatedRecords.Remove(record);

            cambiosDetalleAgenda.UpdatedRecords.Add(detalleAgenda);

        }
        public virtual void UpdateRecordDetalleAgendaProblema(EntityChanges<AgendaDetalleProblema> cambiosProblemas, AgendaDetalleProblema problema)
        {
            var record = cambiosProblemas.UpdatedRecords.FirstOrDefault(d => d.Id == problema.Id);

            if (record != null)
                cambiosProblemas.UpdatedRecords.Remove(record);

            cambiosProblemas.UpdatedRecords.Add(problema);

        }

    }
}
