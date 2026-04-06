namespace WIS.Domain.Documento
{
    public class TotalesDocumento
    {
        public string NumeroDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public long Lineas { get; set; }
        public long Productos { get; set; }
        public decimal Desafectada { get; set; }
        public decimal CIF { get; set; }
    }
}
