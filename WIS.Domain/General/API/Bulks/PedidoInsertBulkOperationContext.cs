using System.Collections.Generic;
using WIS.Domain.Picking;

namespace WIS.Domain.General.API.Bulks
{
    public class PedidoInsertBulkOperationContext
    {
        public List<object> NewPedidos = new List<object>();

        public List<DetallePedido> NewLineas = new List<DetallePedido>();

        public List<DetallePedidoDuplicado> NewDuplicados = new List<DetallePedidoDuplicado>();

        public List<DetallePedidoLpn> NewDetalleLpn = new List<DetallePedidoLpn>();

        public List<DetallePedidoAtributos> NewAtributos = new List<DetallePedidoAtributos>();

        public List<DetallePedidoAtributosLpn> NewAtributosLpn = new List<DetallePedidoAtributosLpn>();

        public List<DetallePedidoConfigAtributo> NewAtributosDetalle = new List<DetallePedidoConfigAtributo>();
    }
}
