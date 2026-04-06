using System.ComponentModel;

namespace WIS.Domain.Integracion
{
    public enum ServiceHttpProtocol
    {
        [Description("Post")]
        POST,
        [Description("Get")]
        GET,
        [Description("Delete")]
        DELETE,
        [Description("Put")]
        PUT,
        [Description("Desconocido")]
        UNKNOWN
    }
}
