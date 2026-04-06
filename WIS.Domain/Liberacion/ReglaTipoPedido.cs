using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Liberacion
{
    public class ReglaTipoPedido
    {
        public int nuRegla { get; set; }

        public string tpPedido { get; set; }

        public DateTime? dtAddRow { get; set; }

        public DateTime? dtUpdRow { get; set; }

        public virtual ReglaLiberacion ReglaLiberacion { get; set; }

    }
}
