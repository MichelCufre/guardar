using System.Collections.Generic;

namespace WIS.Domain.General.API.Bulks
{
    public class AgendaBulkOperationContext
    {
        public List<object> NewAgendas = new List<object>();

        public List<object> NewAgendaReferencias = new List<object>();

        // Para cargar las agendas relevantes para la insercion a T_RECEPCION_AGENDA_PROBLEMA
        public List<object> NewAgendaLiberadaReferencias = new List<object>();
    }
}
