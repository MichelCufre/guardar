using System.Collections.Generic;

namespace WIS.Domain.General.API.Bulks
{
    public class PedidoAnuladoBulkOperationContext
    {
        public List<object> NewDuplicados = new List<object>();
        public List<object> UpdateDuplicados = new List<object>();
        public List<object> UpdateLogPedidoAnulado = new List<object>();
        public List<long> NuLogsPedidoAnulado = new List<long>();
    }
}
