using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Porteria
{
    public class PorteriaSector
    {
        public string CD_SECTOR { get; set; }

        public string DS_SECTOR { get; set; }

        public string NU_PREDIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public short? CD_PORTA { get; set; }

    }
}
