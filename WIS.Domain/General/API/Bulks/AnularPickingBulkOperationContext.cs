using System.Collections.Generic;

namespace WIS.Domain.General.API.Bulks
{
    public class AnularPickingBulkOperationContext
    {
        public List<object> InsertAnulacionPreparacion = new List<object>();
        public List<object> UpdatePickingPedidoPendiente = new List<object>();
        public List<object> InsertPickingPedidoPendienteAnular = new List<object>();
    }
}
