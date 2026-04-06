using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class PreferenciaAsociarCliente
    {
        public int nuPreferencia { get; set; }
        public string cdCliente { get; set; }
        public string dsCliente { get; set; }
        public int cdEmpresa { get; set; }
        public string nmEmpresa { get; set; }
    }
}
