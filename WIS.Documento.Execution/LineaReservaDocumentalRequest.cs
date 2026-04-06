namespace WIS.Documento.Execution
{
    public class LineaReservaDocumentalRequest
    {
        public string Producto { get; set; }
        public string Identificador { get; set; }
        public decimal? CantidadAfectada { get; set; }
        public int Empresa { get; set; }
        public int Preparacion { get; set; }
        public decimal? Faixa { get; set; }
        public string EspecificaIdentificador { get; set; }
        public string Semiacabado { get; set; }
        public string Consumible { get; set; }
    }
}
