using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Registro
{
   public class ProductoProveedorObjeto
    {
        public string CD_PRODUTO { get; set; }
        public string DS_PRODUTO { get; set; }
        public int CD_EMPRESA { get; set; }
        public string NM_EMPRESA { get; set; }
        public string DS_CLIENTE { get; set; }
        public string CD_AGENTE { get; set; }
        public string TP_AGENTE { get; set; }
        public string CD_EXTERNO { get; set; }
        public string CD_CLIENTE { get; set; }
    }
}
