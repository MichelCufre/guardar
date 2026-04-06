using System.Collections.Generic;
using WIS.Domain.Picking;

namespace WIS.Domain.General.API.Bulks
{
    public class PedidoUpdateBulkOperationContext
    {
        public List<object> UpdPedidos = new List<object>();

        public List<DetallePedido> UpdDetalles = new List<DetallePedido>();
        public List<DetallePedido> NewDetalles = new List<DetallePedido>();

        public List<DetallePedidoDuplicado> UpdDuplicados = new List<DetallePedidoDuplicado>();
        public List<DetallePedidoDuplicado> NewDuplicados = new List<DetallePedidoDuplicado>();

        public List<DetallePedidoLpn> NewDetalleLpn = new List<DetallePedidoLpn>();
        public List<DetallePedidoLpn> UpdateDetalleLpn = new List<DetallePedidoLpn>();
        public List<DetallePedidoLpn> DeleteDetalleLpn = new List<DetallePedidoLpn>();

        public List<DetallePedidoAtributos> NewAtributos = new List<DetallePedidoAtributos>();
        public List<DetallePedidoAtributos> UpdateAtributos = new List<DetallePedidoAtributos>();
        public List<DetallePedidoAtributos> DeleteAtributos = new List<DetallePedidoAtributos>();

        public List<DetallePedidoAtributosLpn> NewAtributosLpn = new List<DetallePedidoAtributosLpn>();
        public List<DetallePedidoAtributosLpn> UpdateAtributosLpn = new List<DetallePedidoAtributosLpn>();
        public List<DetallePedidoAtributosLpn> DeleteAtributosLpn = new List<DetallePedidoAtributosLpn>();

        public List<DetallePedidoConfigAtributo> NewAtributosDetalle = new List<DetallePedidoConfigAtributo>();
        public List<DetallePedidoConfigAtributo> DeleteAtributosDetalle = new List<DetallePedidoConfigAtributo>();
    }
}
