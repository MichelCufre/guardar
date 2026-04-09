namespace Custom.Domain.Services.Configuration
{
    public class ErpClientSettings
    {
        public const string Position = "ErpClientSettings";

        public string WsGenQueryUrl { get; set; }   // URL del web service SOAP del cliente
        public int Empresa          { get; set; }   // Codigo de empresa a usar en los requests WMS
        public string Referencia    { get; set; }   // Descripcion de referencia para la interfaz
    }
}
