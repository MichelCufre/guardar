using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class PedidoProductoContenedor
    {
        public int? NumeroContenedor { get; set; }
        public int NumeroPreparacion { get; set; }
        public int Empresa { get; set; }
        public string CodigoProducto { get; set; }
        public string CodigoCliente { get; set; }
        public string NumeroPedido { get; set; }
        public string ComparteContenedorEntrega { get; set; }
        public string DireccionEntrega { get; set; }
        public string TipoExpedicion { get; set; }
        public decimal? CantidadProducto { get; set; }
    }
}
