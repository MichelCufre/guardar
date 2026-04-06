using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.Porteria
{
    public class PorteriaRegistroVehiculo
    {
        public PorteriaRegistroVehiculo()
        {
            Personas = new List<PorteriaRegistroPersona>();
        }

        public int NU_PORTERIA_VEHICULO { get; set; }

        public string ND_TRANSPORTE { get; set; }

        public string VL_MATRICULA_1 { get; set; }

        public string VL_MATRICULA_2 { get; set; }

        public decimal? VL_PESO_ENTRADA { get; set; }

        public decimal? VL_PESO_SALIDA { get; set; }

        public int? CD_EMPRESA { get; set; }

        public DateTime? DT_PORTERIA_ENTRADA { get; set; }

        public DateTime? DT_PORTERIA_SALIDA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public string FL_SOLO_BALANZA { get; set; }

        public int? NU_EJECUCION_ENTRADA { get; set; }

        public int? NU_EJECUCION_SALIDA { get; set; }

        public string FL_SALIDA_HABILITADA { get; set; }

        public string FL_CONTROL_SALIDA { get; set; }

        public string ND_TP_FACTURACION { get; set; }

        public List<PorteriaRegistroPersona> Personas { get; set; }
        public string CD_AGENTE { get; set; }
        public string NU_PREDIO { get; set; }
        public string CD_SECTOR { get; set; }
        public string ND_POTERIA_MOTIVO { get; set; }
        public string ND_SECTOR { get; set; }
        public string TP_AGENTE { get; set; }
    }
}
