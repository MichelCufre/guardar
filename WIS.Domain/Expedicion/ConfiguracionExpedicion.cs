using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class ConfiguracionExpedicion
    {
        public bool IsControlFacturacionRequerido { get; set; }
        public bool IsSincronizacionTrackingHabilitada { get; set; }
        public bool PermiteAsociarDisitntosGruposExpedicion { get; set; }
    }
}
