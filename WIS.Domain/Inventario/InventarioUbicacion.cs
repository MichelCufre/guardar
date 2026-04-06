using System.Collections.Generic;

namespace WIS.Domain.Inventario
{
    public class InventarioUbicacion
    {
        public decimal Id { get; set; }

        public decimal IdInventario { get; set; }

        public string Ubicacion { get; set; }

        public string Estado { get; set; }

        public long? NumeroTransaccion { get; set; }

        public long? NumeroTransaccionDelete { get; set; }

        public List<InventarioUbicacionDetalle> Detalles { get; set; }

        public InventarioUbicacion()
        {
            this.Detalles = new List<InventarioUbicacionDetalle>();
        }
    }
}
