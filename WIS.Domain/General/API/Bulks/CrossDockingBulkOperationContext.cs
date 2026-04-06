using System.Collections.Generic;
using WIS.Domain.Recepcion;

namespace WIS.Domain.General.API.Bulks
{
    public class CrossDockingBulkOperationContext
    {
        public List<Agenda> AgendasUpdate = new List<Agenda>();

        public List<AgendaDetalle> UpdateDetalleAgenda = new List<AgendaDetalle>();

        public List<object> NewStock = new List<object>();

        public List<object> UpdateStock = new List<object>();

        public List<object> EtiquetaLote = new List<object>();

        public List<object> Contenedores = new List<object>();

        public List<object> InsertDetallesEtiquetaLote = new List<object>();

        public List<object> UpdateDetallesEtiquetaLote = new List<object>();
    }
}
