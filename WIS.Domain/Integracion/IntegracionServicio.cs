using System;
using WIS.Domain.Integracion.Authentication;

namespace WIS.Domain.Integracion
{
    public class IntegracionServicio
    {
        public string UrlIntegracion { get; set; }

        public bool Habilitado { get; set; }

        public string TipoAutenticacion { get; set; }

        public string TipoComunicacion { get; set; }

        public string User { get; set; }

        public string Secret { get; set; }

        public string Scope { get; set; }

        public string UrlAuthServer { get; set; }

        public int Numero { get; set; }

        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public string SecretSalt { get; set; }

        public decimal? SecretFormat { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? Transaccion { get; set; }

        public IAuthenticationMethod Authorization { get; set; }
    }
}
