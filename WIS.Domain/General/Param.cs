using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
   public class Param
    {
        public string CodigoAplicacion { get; set; }
        public string TipoAplicacion { get; set; }
        public string CodigoParametro { get; set; }
        public string ValorParametro { get; set; }
        public string DescripcionParametro { get; set; }
        public string DisponiblePorEmpresa { get; set; }
    }
}
