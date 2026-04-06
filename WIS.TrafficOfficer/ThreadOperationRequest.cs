using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.TrafficOfficer
{
    public class ThreadOperationRequest
    {
        public int? Userid { get; set; }
        public string Pagina { get; set; }
        public string Sistema { get; set; }
        public bool Mono_hilo { get; set; }
    }
}
