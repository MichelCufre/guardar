using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Picking;

namespace WIS.Domain.General.API.Bulks
{
    public class EgresoBulkOperationContext
    {
        public List<object> NewCargas = new List<object>();
        public List<object> NewEgresos = new List<object>();
        public List<Pedido> UpdatePedidos = new List<Pedido>();
        public List<DetallePreparacion> UpdateDetallesPickingPedido = new List<DetallePreparacion>();
        public List<DetallePreparacion> UpdateDetallesPickingContenedor = new List<DetallePreparacion>();
        public List<object> DeleteCargasCamion = new List<object>();
        public Dictionary<string, object> NewCargasCamion = new Dictionary<string, object>();
    }

    public class DTOPikcingCantidades
    {
        public decimal CantidadProducto { get; set; }
        public decimal CantidadPreparada { get; set; }
    }

    public class BulkUpdatePedidosContext
    {
        public List<object> Pedidos = new List<object>();
    }

    public class BulkUpdateEntregasContext
    {
        public List<object> Entregas = new List<object>();
    }

    public class BulkUpdateDuplicadosContext
    {
        public List<object> NewDuplicados = new List<object>();

        public Dictionary<string, object> UpdateDuplicados = new Dictionary<string, object>();
    }

    public class BulkCamionEjecucionContext
    {
        public List<object> Datos = new List<object>();
    }
}
