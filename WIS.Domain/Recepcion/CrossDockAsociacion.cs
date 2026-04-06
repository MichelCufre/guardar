using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Picking;

namespace WIS.Domain.Recepcion
{
    public class CrossDockAsociacion
    {
        public Agenda Agenda { get; set; }
        public List<Preparacion> Preparaciones { get; set; }
    }
}
