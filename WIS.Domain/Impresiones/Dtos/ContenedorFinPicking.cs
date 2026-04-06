using System;

namespace WIS.Domain.Impresiones.Dtos
{
    public class ContenedorFinPicking
    {
        public string Anexo1 { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string NumeroPedido { get; set; }
        public string CodigoCliente { get; set; }
        public string DescripcionCliente { get; set; }
        public string DescripcionUbicacion { get; set; }
        public string Anexo4 { get; set; }
        public string DescripcionRuta { get; set; }
        public string TotalBultos { get; set; }
        public string TipoContenedor { get; set; }
        public int NumeroContenedor { get; set; }
        public int NumeroPreparacion { get; set; }
        public string IdExterno { get; set; }
        public string CodigoBarras { get; set; }
        public string DescripcionContenedor { get; set; }
    }
}
