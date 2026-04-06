using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.StockEntities
{
    public class LpnDetalleDisponible
    {
        public long Lpn { get; set; }
        public int DetalleLpn { get; set; }
        public decimal? Disponible { get; set; }
        public string JsonAtributos { get; set; }
    }
}
