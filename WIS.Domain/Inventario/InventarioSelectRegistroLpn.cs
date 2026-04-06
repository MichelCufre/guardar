using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Inventario
{
    public class InventarioSelectRegistroLpn
    {
        public decimal NuInventario { get; set; }
        public decimal NuInventarioUbicacion { get; set; }
        public decimal NuInventarioUbicacionDetalle { get; set; }

        public string Ubicacion { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime? Vencimiento { get; set; }
        public string NroLpn { get; set; }
        public long? NroLpnReal { get; set; }
        public string IdDetalleLpn { get; set; }
        public int? IdDetalleLpnReal { get; set; }

        public long? IdOperacion { get; set; }

        public long? NuInstanciaConteo { get; set; }
        public decimal? NuConteo { get; set; }
    }
}
