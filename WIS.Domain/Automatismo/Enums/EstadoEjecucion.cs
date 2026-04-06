using System.ComponentModel;

namespace WIS.Domain.Automatismo.Enums
{
    public enum EstadoEjecucion
    {
        [Description("Pendiente")]
        PENDIENTE,
        [Description("Procesado sin errores")]
        PROCESADO_OK,
        [Description("Procesado con errores en la API")]
        PROCESADO_ERROR_API,
        [Description("Procesado con errores en el Automatismo")]
        PROCESADO_ERROR_AUTOMATISMO,
        [Description("Desconocido")]
        UNKNOWN,
        [Description("Interfaz reprocesada")]
        ESTPROCREP,
        [Description("Interfaz Procesada Finalizada")]
        PROCESADO_FIN,
    }
}
