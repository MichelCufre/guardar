using System.Collections.Generic;

namespace WIS.Domain.General.API.Bulks
{
    public class AgenteBulkOperationContext
    {
        public IEnumerable<Agente> NewAgentes = new List<Agente>();

        public List<object> UpdAgentes = new List<object>();

        public List<ClienteDiasValidezVentana> NewClientesiasValidezVentana { get; set; }
    }
}
