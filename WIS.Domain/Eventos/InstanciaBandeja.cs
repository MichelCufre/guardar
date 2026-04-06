using System;
using System.Collections.Generic;

namespace WIS.Domain.Eventos
{

    public partial class InstanciaBandeja
    {
        public InstanciaBandeja()
        {
            //  Instancia = new List<Instancia>();
        }
        public int NU_EVENTO_BANDEJA_INSTANCIA { get; set; }

        public int NU_EVENTO_BANDEJA { get; set; }

        public int NU_EVENTO_INSTANCIA { get; set; }

        public EstadoBandeja ND_ESTADO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public Bandeja Bandeja { get; set; }

        public Instancia Instancia { get; set; }
    }
}
