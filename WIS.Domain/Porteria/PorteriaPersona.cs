using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Porteria
{
    public class PorteriaPersona
    {
        public int NU_POTERIA_PERSONA { get; set; }

        public string NU_DOCUMENTO { get; set; }

        public string ND_TP_DOCUMENTO { get; set; }

        public string CD_PAIS_EMISOR { get; set; }

        public string NM_PERSONA { get; set; }

        public string AP_PERSONA { get; set; }

        public string NU_CELULAR { get; set; }

        public string ND_TP_PERSONA { get; set; }

        public string ND_PUESTO_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? CD_EMPRESA { get; set; }

        public string CD_PORTERIA_EMPRESA { get; set; }

    }
}
