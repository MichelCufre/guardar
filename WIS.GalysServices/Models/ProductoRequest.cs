namespace WIS.GalysServices.Models
{
    public class ProductoRequest
    {
        public string CodAlmacen { get; set; }
        public string CodArticulo { get; set; }
        public string DenomArticulo { get; set; }
        public string Estado { get; set; }
        public bool GestionLoteEntrada { get; set; }
        public bool GestionCaducidadEntrada { get; set; }
        public bool LeerCdBSalidas { get; set; }
        public decimal? Peso { get; set; }
        public decimal? DimensionXEnvase { get; set; }
        public decimal? DimensionYEnvase { get; set; }
        public decimal? DimensionZEnvase { get; set; }
        public string Udc { get; set; }
        public int? UdsUdc { get; set; }
        public decimal? UdsUde { get; set; }
    }
}
