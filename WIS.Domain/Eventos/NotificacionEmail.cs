using System;

namespace WIS.Domain.Eventos
{
    public partial class NotificacionEmail : Notificacion
    {

        public DateTime? FechaEnvio { get; set; }

        public string Cuerpo { get; set; }

        public DateTime? FechaRenvio { get; set; }

        public string EmailRecibe { get; set; }

        public string EmailEnvia { get; set; }

        public string Asunto { get; set; }

        public EstadoNotificacion Estado { get; set; }

        public DateTime? FechaCreacion { get; set; }
        
        public bool? IsHtml { get; set; }

        public NotificacionEmail()
        {
            this.TipoNotificacion = TipoNotificacion.EMAIL;
        }
    }
}
