using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Expedicion;
using WIS.Domain.General;

namespace WIS.Domain.Tracking.Models
{
    public class EgresoContenedorTracking
    {
        public Camion Egreso { get; set; }
        public Contenedor Contenedor { get; set; }
        public bool Baja { get; set; }
    }
}
