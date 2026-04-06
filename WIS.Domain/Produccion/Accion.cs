using WIS.Domain.Produccion.Enums;

namespace WIS.Domain.Produccion
{
    public class Accion
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public AccionTipo Tipo { get; set; }
    }
}
