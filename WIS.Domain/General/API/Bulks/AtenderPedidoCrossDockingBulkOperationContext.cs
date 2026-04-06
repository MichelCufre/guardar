using System.Collections.Generic;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;

namespace WIS.Domain.General.API.Bulks
{
    public class AtenderPedidoCrossDockingBulkOperationContext
    {

        public List<CrossDockingTemp> NewCrossDockingTemp;

        public List<DetallePedido> NewDetallePedido;

        public List<DetallePedido> UpdateDetallePedido;

        public List<AgendaDetalle> UpdateDetalleAgenda;

        public AtenderPedidoCrossDockingBulkOperationContext()
        {
            NewDetallePedido = new List<DetallePedido>();
            UpdateDetallePedido = new List<DetallePedido>();
            UpdateDetalleAgenda = new List<AgendaDetalle>();
            NewCrossDockingTemp = new List<CrossDockingTemp>();
        }
    }
}
