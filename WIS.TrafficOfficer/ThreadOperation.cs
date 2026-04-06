using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.TrafficOfficer
{
    public class ThreadOperation
    {
        public string Token_thread { get; set; }
        public string Transaccion { get; set; }
        public bool TooManySessions { get; set; }

        public List<ItemLock> colItemLock { get; set; }
        public List<ItemThread> colItemThread { get; set; }
    }
}
