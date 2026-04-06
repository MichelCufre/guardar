using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
   public class ContenedorExpedir
    {
        public int NumeroContenedor { get; set; }
        public int NumeroPreparacion { get; set; }
        public long NumeroCarga { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CodigoCliente { get; set; }

    }
}
