using System.ComponentModel;

namespace WIS.Domain.Eventos
{
    public enum TipoNotificacion
    {
        [Description("Notificacion por Email")]
        EMAIL,
        [Description("Inventario desconocido")]
        Unknown
    }

    public static class TipoNotificacionHelper
    {
        public static TipoNotificacion GetTipoNotificacion(string value)
        {
            switch (value)
            {
                case "EMAIL": return TipoNotificacion.EMAIL;
                default: return TipoNotificacion.Unknown;
            }
        }
    }
}
