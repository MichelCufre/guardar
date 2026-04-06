using System.ComponentModel;

namespace WIS.Domain.Eventos
{
    public enum EstadoBandeja
    {
        [Description("Pendiente")]
        EST_PEND,
        [Description("Termina con errores")]
        EST_CON_ERRORES,
        EST_FIN_CORRECTO,
        EST_SIN_LOGIC_EVENTO,
        EST_INST_DESHABILITADA,
        Unknown
    }

    public static class EstadoBandejaHelper
    {
        public static EstadoBandeja GetEstado(string value)
        {
            switch (value)
            {
                case "EST_PEND": return EstadoBandeja.EST_PEND;
                case "EST_CON_ERRORES": return EstadoBandeja.EST_CON_ERRORES;
                case "EST_FIN_CORRECTO": return EstadoBandeja.EST_FIN_CORRECTO;
                case "EST_SIN_LOGIC_EVENTO": return EstadoBandeja.EST_SIN_LOGIC_EVENTO;
                case "EST_INST_DESHABILITADA": return EstadoBandeja.EST_INST_DESHABILITADA;
                default: return EstadoBandeja.Unknown;
            }
        }
    }
}