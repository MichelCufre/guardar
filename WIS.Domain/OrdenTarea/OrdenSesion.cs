using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.OrdenTarea
{
    public class OrdenSesion
    {
        public long NuOrtOrdenSesion { get; set; }
        public int NuOrtOrden { get; set; }
        public int CdFuncionario { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFin { get; set; }

    }
}