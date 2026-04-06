using System.Collections.Generic;
using WIS.Domain.Picking;

namespace WIS.Domain.General.API.Bulks
{
    public class PedidoGenerarDetalleLpnBulkOperationContext
    {
        public Dictionary<string, DetallePedido> NewDetalles = new Dictionary<string, DetallePedido>();
        public Dictionary<string, DetallePedido> UpdateDetalles = new Dictionary<string, DetallePedido>();

        public Dictionary<string, DetallePedidoLpn> NewDetallesLpn = new Dictionary<string, DetallePedidoLpn>();
        public Dictionary<string, DetallePedidoLpn> UpdateDetallesLpn = new Dictionary<string, DetallePedidoLpn>();

        public List<DetallePedidoLpnAtributo> UpdateDetallesLpnAtributo = new List<DetallePedidoLpnAtributo>();
        public List<DetallePedidoAtributoDefinicion> UpdateDefinicionAtributos = new List<DetallePedidoAtributoDefinicion>();
    }
}
