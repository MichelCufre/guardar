using System.ComponentModel;

namespace WIS.Domain.Eventos
{
    public enum EstadoNotificacion
    {
        EST_PEND,
        EST_FIN_CORRECTO,
        EST_CON_ERRORES,
        Unknown
    }

    public static class EstadoNotificacionHelper
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