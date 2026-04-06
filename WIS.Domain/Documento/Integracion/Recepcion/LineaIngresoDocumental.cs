namespace WIS.Documento.Execution
{
    public class LineaIngresoDocumental
    {
        public string Producto { get; set; }
        public string Identificador { get; set; }
        public decimal? CantidadAfectada { get; set; }
        public int Empresa { get; set; }
        public string Semiacabado { get; set; }
        public decimal? Faixa { get; set; }
    }
}
