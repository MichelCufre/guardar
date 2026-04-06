namespace WIS.Domain.Integracion.Dtos
{
    public class IntegracionServicioConexionRequest
    {
        public ServiceHttpProtocol ProtocoloComunicacion { get; set; }
        public int InterfazExterna { get; set; }
        public int? Interfaz { get; set; }
        public string Url { get; set; }
        public string Contenido { get; set; }
    }
}
