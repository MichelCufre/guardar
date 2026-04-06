using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.TrafficOfficer
{
    public class ItemThread
    {
        public int? Userid { get; set; }
        public string Pagina { get; set; }
        public string Sistema { get; set; }
        public bool Mono_hilo { get; set; }
        public string Token_thread { get; set; }
        public string UltimaActividad { get; set; }
        public string Ingreso { get; set; }
    }
}
