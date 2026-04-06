using System.Collections.Generic;
using WIS.Domain.Picking;

namespace WIS.Domain.General.API.Bulks
{
    public class PedidoAnuladoLpnBulkOperationContext
    {
        public List<PedidoAnuladoLpn> NewLogPedidoAnuladoLpn = new List<PedidoAnuladoLpn>();
        public List<DetallePedidoLpn> UpdateDetallePedidoLpn = new List<DetallePedidoLpn>();
        public List<DetallePedidoAtributo> UpdateDetallePedidoAtributo = new List<DetallePedidoAtributo>();
        public List<DetallePedidoLpnAtributo> UpdateDetallePedidoLpnAtributo = new List<DetallePedidoLpnAtributo>();

        public PedidoAnuladoLpnBulkOperationContext()
        {
            NewLogPedidoAnuladoLpn = new List<PedidoAnuladoLpn>();
            UpdateDetallePedidoLpn = new List<DetallePedidoLpn>();
            UpdateDetallePedidoAtributo = new List<DetallePedidoAtributo>();
            UpdateDetallePedidoLpnAtributo = new List<DetallePedidoLpnAtributo>();
        }
    }
}
