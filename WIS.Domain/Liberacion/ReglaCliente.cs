using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Liberacion
{
    public class ReglaCliente
    {
        public int NuRegla { get; set; }

        public string Cliente { get; set; }

        public int Empesa { get; set; }

        public short? NuOrden { get; set; }

        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public virtual ReglaLiberacion ReglaLiberacion { get; set; }
    }
}
