using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.OrdenTarea
{
    public class OrdenSesionEquipo
    {
        public long NuOrtOrdenSesionEquipo { get; set; }
        public long NuOrtOrdenSesion { get; set; }
        public int CdEquipo { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFin { get; set; }
    }
}
