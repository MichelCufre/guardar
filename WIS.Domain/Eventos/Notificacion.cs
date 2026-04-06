using System.Collections.Generic;

namespace WIS.Domain.Eventos
{
    public abstract partial class Notificacion
    {

        public Notificacion()
        {
            this.Instancia = new Instancia();
            this.Archivos = new List<NotificacionArchivo>();
        }

        public long Id { get; set; }

        public int NumeroInstancia { get; set; }

        public Instancia Instancia { get; set; }

        public TipoNotificacion TipoNotificacion { get; set; }

        public List<NotificacionArchivo> Archivos { get; set; }

        public static TipoNotificacion GetTipoNotificacion(string value) 
        {
            switch (value) //TPEVNOT
            {
                case "EMAIL": return TipoNotificacion.EMAIL;
                default: return TipoNotificacion.Unknown;
            }
        }

        public static string GetTipoNotificacion(TipoNotificacion value)
        {
            switch (value)
            {
                case TipoNotificacion.EMAIL: return "EMAIL";
                default: return "Unknown";
            }
        }

        public static EstadoNotificacion GetEstado(string value)
        {
            switch (value)//NDEVNOT
            {
                default: return EstadoNotificacion.Unknown;
            }
        }

        /*public static string GetEstado(EstadoNotificacion value)
        {
            switch (EstadoNotificacion.EST_PEND)
            {


                default: return "Unknown";
            }
        }*/

    }
}
