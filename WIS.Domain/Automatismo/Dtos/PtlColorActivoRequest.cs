namespace WIS.Domain.Automatismo
{
    public class PtlColorActivoRequest
    {
        public int UserId { get; set; }
        public string Usuario { get; set; }
        public string Contenedor { get; set; }
        public string Color { get; set; }
        public long Preparacion { get; set; }
        public string Cliente { get; set; }
        public string ComparteContenedorPicking { get; set; }
        public string SubClase { get; set; }
        public string IdPtl { get; set; }
        public string Serializado { get; set; }
    }
}
