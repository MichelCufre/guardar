namespace WIS.Domain.Services.Configuracion
{
    public class AnulacionDocumentalSettings
    {
        public const string Position = "AnulacionDocumentalSettings";

        public int CantidadAnulaciones { get; set; }
        public string MutexId { get; set; }
        public int? MutexTimeout { get; set; }
    }
}
