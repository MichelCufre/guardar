namespace WIS.Domain.Integracion.Dtos
{
    public class AutomatismoInterpreterRequest
    {
        public IntegracionServicioConfigRequest IntegracionServicio { get; set; }
        public IntegracionServicioConexionRequest IntegracionServicioConexion { get; set; }
    }
}
