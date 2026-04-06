using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.StockEntities
{
    public class TraspasoEmpresasPreparacionPreparado
    {
        public int Preparacion { get;  set; }
        public string Descripcion { get;  set; }
        public int? Empresa { get;  set; }
        public int? CantidadPendiente { get;  set; }
        public int? CantidadPreparada { get;  set; }
        public decimal? CantidadSaldoSinTrabajar { get;  set; }
        public int? CantidadPedidoNoTraspaso { get;  set; }
    }
}
