using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class AgenteRutaPredio
    {
        public int Empresa { get; set; }
        public string CodigoInternoAgente { get; set; }
        public short Ruta { get; set; }
        public string Predio { get; set; }
    }
}
