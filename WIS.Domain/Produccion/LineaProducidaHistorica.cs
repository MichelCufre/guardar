using System;

namespace WIS.Domain.Produccion
{
    public class LineaProducidaHistorica
    {
        public long NumeroHistorico { get; set; }
        public int Iteracion { get; set; }
        public int Pasada { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public decimal? Cantidad { get; set; }
        public DateTime? FechaProducido { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaAlta { get; set; }
    }
}
