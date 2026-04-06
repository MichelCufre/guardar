using System.Collections.Generic;

namespace WIS.Domain.Picking.Dtos
{
    public class AnularPickingPedidoPendienteAutomatismo
    {
        public AnularPickingPedidoPendienteAutomatismo()
        {
            Detalle = new List<AnularPickingPedidoPendienteDetalle>();
        }

        public string Pedido { get; set; }
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public string Cliente { get; set; }
        public int Preparacion { get; set; }
        public int Empresa { get; set; }
        public string EstadoPicking { get; set; }
        public long NuTransaccion { get; set; }
        public int NroAnulacionPreparacion { get; set; }
        public string EstadoAnulacion { get; set; }
        public long? Carga { get; set; }
        public string ComparteContenedorPicking { get; set; }

        public List<AnularPickingPedidoPendienteDetalle> Detalle { get; set; }
    }

    public class AnularPickingPedidoPendienteDetalle
    {
        public string CodigoProducto { get; set; }
        public string Identificador { get; set; }
        public decimal CantidadAnular { get; set; }
    }

}
