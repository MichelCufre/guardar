using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Exceptions;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class CrearDetalleAgendaMultiSeleccionConFacturaStrategy : ICrearDetallesAgendaStrategy
    {
        protected IUnitOfWork _uow;
        protected List<int> _referencias;
        protected Agenda _agenda;

        public CrearDetalleAgendaMultiSeleccionConFacturaStrategy(IUnitOfWork uow, Agenda agenda, List<int> keyReferencias)
        {
            _uow = uow;
            _referencias = keyReferencias;
            _agenda = agenda;
        }

        /// <summary>
        /// + Crea asociación de cabezales Agenda-Referencias
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual List<AgendaDetalle> CrearDetallesAgenda()
        {

            foreach (var key in _referencias)
            {
                var referencia = _uow.ReferenciaRecepcionRepository.GetReferencia(key);

                if (referencia == null)
                    throw new EntityNotFoundException("General_Sec0_Error_NoSeEncontroReferencia", new string[] { key.ToString() });

                _uow.ReferenciaRecepcionRepository.AsociarReferenciaAgenda(_agenda.Id, key);

            }
            // Las agendas se crean al ingresar las linas de facuras.

            return new List<AgendaDetalle>();
        }
    }
}
