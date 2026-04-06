using System.Collections.Generic;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class CrearDetalleAgendaLibreStrategy : ICrearDetallesAgendaStrategy
    {
        protected List<AgendaDetalle> _detalles;

        public CrearDetalleAgendaLibreStrategy()
        {

        }

        public virtual List<AgendaDetalle> CrearDetallesAgenda()
        {
            return new List<AgendaDetalle>();
        }
    }
}
