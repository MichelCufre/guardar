using System.Collections.Generic;

namespace WIS.Documento.Execution
{
    public class LineaEgresoDocumentalRequest
    {
        public string Producto { get; set; }
        public string Identificador { get; set; }
        public decimal? CantidadAfectada { get; set; }
        public int Empresa { get; set; }
        public int? Preparacion { get; set; }
        public string Semiacabado { get; set; }
        public string Consumible { get; set; }
        public decimal? Faixa { get; set; }
    }
}
