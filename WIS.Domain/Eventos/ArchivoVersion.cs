using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Eventos
{
    public class ArchivoVersion
    {
        public string LK_RUTA { get; set; }
        public string TP_ARCHIVO { get; set; }
        public long NU_VERSION { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public int CD_FUNCIONARIO { get; set; }

        public string SUB_LINK { get; set; }
    }
}
