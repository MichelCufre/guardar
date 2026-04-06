namespace WIS.Domain.Automatismo
{
    public class ProductoZonaAutomatismo
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal? PesoBruto { get; set; }
        public decimal? Ancho { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Profundidad { get; set; }
        public string ManejoIdentificador { get; set; }
        public string TipoManejoFecha { get; set; }
        public string Zona { get; set; }
        public string TipoOperacion { get; set; }
        public string ConfirmarCodigoBarras { get; set; }
        public string UnidadCaja { get; set; }
        public int? CantidadUnidadCaja { get; set; }
        public decimal? UnidadBulto { get; set; }
    }
}
