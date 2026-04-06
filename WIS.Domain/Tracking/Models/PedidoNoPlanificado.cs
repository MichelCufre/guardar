using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PedidoNoPlanificado
    {
        public string Pedido { get; set; }
        public int Empresa { get; set; }
        public string CodigoCliente { get; set; }
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public DateTime? FechaEmitido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string PuntoEntrega { get; set; }
        public string ComparteContedorPicking { get; set; }
        public string ComparteContedorEntrega { get; set; }
    }
}
