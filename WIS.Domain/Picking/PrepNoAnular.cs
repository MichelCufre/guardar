using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
   public class PrepNoAnular
    {
        public int cdEmpresa { get; set; }

        public int nuPreparacion { get; set; }

        public string nuPedido { get; set; }

        public string cdCliente { get; set; }

        public DateTime? dtFin { get; set; }
    }
}
