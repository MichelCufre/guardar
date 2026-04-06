using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class TipoDeEstructura
    {
        public short Codigo { get; set; }
        public string Tipo { get; set; }
        public string idBloqueado { get; set; }
        public string idPortaPallet { get; set; }
        public string idPrateleria { get; set; }
        public string idGailoa { get; set; }

    }
}