namespace WIS.Domain.Services.Configuracion
{
    public class AnulacionSettings
    {
        public const string Position = "AnulacionSettings";

        public int CantidadAnulaciones { get; set; }
        public string MutexId { get; set; }
        public int? MutexTimeout { get; set; }
    }
}
