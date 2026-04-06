using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class PedidoLoteContenedor
    {
        public int? NumeroContenedor { get; set; }
        public int NumeroPreparacion { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CodigoCliente { get; set; }
        public string NumeroPedido { get; set; }
        public string DescripcionUbicacion { get; set; }
        public string ComparteContenedorPicking { get; set; }
        public string ComparteContenedorEntrega { get; set; }
        public string CodigoProducto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public decimal? CantidadProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Anexo4 { get; set; }
    }
}
