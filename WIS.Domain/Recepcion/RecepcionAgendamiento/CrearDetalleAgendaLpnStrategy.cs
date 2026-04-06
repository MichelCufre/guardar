using System.Collections.Generic;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class CrearDetalleAgendaLpnStrategy : ICrearDetallesAgendaStrategy
    {

        protected List<AgendaDetalle> _detalles;

        public CrearDetalleAgendaLpnStrategy()
        {

        }

        public virtual List<AgendaDetalle> CrearDetallesAgenda()
        {
            return new List<AgendaDetalle>();
        }
    }
}
