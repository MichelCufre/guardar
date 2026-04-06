using System.Collections.Generic;
using WIS.Domain.Picking;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Models;

namespace WIS.Domain.General.API.Bulks
{
    public class ProduccionBulkOperationContext
    {
        public List<IngresoProduccion> NewIngresos = new List<IngresoProduccion>();
        public List<IngresoProduccionDetalleTeorico> NewDetallesTeoricos = new List<IngresoProduccionDetalleTeorico>();

        public List<Preparacion> NewPreparacion = new List<Preparacion>();
        public List<Pedido> NewPedidos = new List<Pedido>();
        public List<DetallePedido> NewDetallesPedidos = new List<DetallePedido>();
    }
}
