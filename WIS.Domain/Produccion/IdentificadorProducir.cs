using System;

namespace WIS.Domain.Produccion
{
    public class IdentificadorProducir
    {
        public string Ubicacion { get; set; }
        public string Producto { get; set; }
        public int Empresa { get; set; }
        public decimal Faixa { get; set; }
        public long Orden { get; set; }
        public string Identificador { get; set; }
        public DateTime? Vencimiento { get; set; }
        public decimal Stock { get; set; }
    }
}
