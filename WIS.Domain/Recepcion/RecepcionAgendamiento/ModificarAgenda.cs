using NLog;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Recepcion.Enums;
using WIS.Exceptions;

namespace WIS.Domain.Recepcion
{
    public class ModificarAgenda
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWork _uow;
        protected string _aplicacion;
        protected int _usuario;
        protected int _numeroAgenda;

        public ModificarAgenda(IUnitOfWork uow, int usuario, string aplicacion, int numeroAgenda)
        {
            this._uow = uow;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._numeroAgenda = numeroAgenda;
        }

        /// <summary>
        /// Actualiza el estado de la agenda Con / Sin diferencia dependiendo si hay problemas registrados
        /// </summary>
        /// <returns> true/false si hay problemas</returns>
        public virtual bool ActualizarEstadoAgendaConSinDiferencia()
        {
            var agenda = _uow.AgendaRepository.GetAgenda(_numeroAgenda);

            if (agenda == null)
                throw new ValidationFailedException("General_Sec0_Error_AgendaNoEncontrada", new string[] { _numeroAgenda.ToString() });

            var conProblema = _uow.AgendaRepository.AnyProblemaAgenda(_numeroAgenda);

            agenda.Estado = (conProblema ? EstadoAgenda.ConferidaConDiferencias : EstadoAgenda.ConferidaSinDiferencias);
            agenda.NumeroTransaccion = _uow.GetTransactionNumber();

            _uow.AgendaRepository.UpdateAgenda(agenda);

            return conProblema;
        }

        public virtual void ActualizarEstadoAgendaSegunSituacionDetalles()
        {
            var agenda = _uow.AgendaRepository.GetAgendaSinDetalles(_numeroAgenda);

            if (agenda == null)
                throw new ValidationFailedException("General_Sec0_Error_AgendaNoEncontrada", new string[] { _numeroAgenda.ToString() });

            logger.Info("ActualizarEstadoAgendaSegunSituacionDetalles() - Agenda: " + agenda.Id);

            List<EstadoAgenda> estadosParaMarcarDiferencia = new List<EstadoAgenda>() {
                EstadoAgenda.Abierta,
                EstadoAgenda.DocumentoAsociado,
                EstadoAgenda.ConferidaConDiferencias
            };
            if (agenda.Estado != EstadoAgenda.Cerrada && agenda.Estado != EstadoAgenda.Cancelada)
            {

                EstadoAgenda estadoNuevo = EstadoAgenda.ConferidaSinDiferencias;

                if (_uow.AgendaRepository.AnyAgendaDetalleConEstados(_numeroAgenda, estadosParaMarcarDiferencia))
                {
                    estadoNuevo = EstadoAgenda.ConferidaConDiferencias;
                }

                logger.Info(" Estado: " + estadoNuevo);

                agenda.Estado = estadoNuevo;
                agenda.NumeroTransaccion = _uow.GetTransactionNumber();

                _uow.AgendaRepository.UpdateAgenda(agenda);
            }
        }

    }
}
