using System;

namespace WIS.Domain.Inventario
{
    public class InventarioStock
    {
        public string Ubicacion { get; set; }
        public string Producto { get; set; }
        public int CodigoEmpresa { get; set; }
        public string DescripcionEmpresa { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? CantidadReservada { get; set; }
        public decimal? CantidadTransitoEntrada { get; set; }
        public string IdAveria { get; set; }
        public DateTime? FechaInventario { get; set; }
        public DateTime? Vencimiento { get; set; }
        public string ControlCalidad { get; set; }
    }
}
