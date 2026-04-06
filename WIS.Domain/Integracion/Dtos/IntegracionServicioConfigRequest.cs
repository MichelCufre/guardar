using WIS.Domain.Integracion.Enums;

namespace WIS.Domain.Integracion.Dtos
{
    public class IntegracionServicioConfigRequest
    {
        public string UrlIntegracion { get; set; }
        public bool Habilitado { get; set; }
        public string TipoAutenticacion { get; set; }
        public string TipoComunicacion { get; set; }
        public string User { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
        public string UrlAuthServer { get; set; }
    }
}
