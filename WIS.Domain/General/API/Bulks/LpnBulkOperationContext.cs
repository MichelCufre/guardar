using System.Collections.Generic;

namespace WIS.Domain.General.API.Bulks
{
    public class LpnBulkOperationContext
    {
        public List<object> NewLpns = new List<object>();
        public List<object> NewLpnDetalles = new List<object>();
        public List<object> NewLpnAtributos = new List<object>();
        public List<object> NewLpnDetalleAtributos = new List<object>();
        public Dictionary<string, object> NewLpnBarras = new Dictionary<string, object>();
    }
}
