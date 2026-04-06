using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.Porteria
{
    public class PorteriaRegistroPersona
    {
        public int NU_PORTERIA_REGISTRO_PERSONA { get; set; }
        public string ND_TP_POTERIA_REGISTRO { get; set; }
        public string ND_POTERIA_MOTIVO { get; set; }
        public int CD_FUNCIONARIO { get; set; }
        public string CD_AGENTE { get; set; }
        public int? NU_POTERIA_PERSONA { get; set; }
        public DateTime? DT_PERSONA_ENTRADA { get; set; }
        public DateTime? DT_PERSONA_SALIDA { get; set; }
        public int? NU_PORTERIA_VEHICULO_ENTRADA { get; set; }
        public int? NU_PORTERIA_VEHICULO_SALIDA { get; set; }
        public string ND_SECTOR { get; set; }
        public string DS_NOTA { get; set; }
        public string ND_ESTADO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public int? CD_EMPRESA { get; set; }
        public string CD_SECTOR { get; set; }
        public string TP_AGENTE { get; set; }
        public string NU_PREDIO { get; set; }
    }
}
