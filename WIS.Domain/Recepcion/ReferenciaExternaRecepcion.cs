using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Recepcion
{
    public class ReferenciaExternaRecepcion
    {
        public int NuRecepcionRelEmpresaTipo { get; set; }
        public string TpRecepcionExterno { get; set; }
        public string DsRecepcionExterno { get; set; }
        public string TpRecepcion { get; set; }
        public string DsTipoRecepcion { get; set; }
        public int? CdEmpresa { get; set; }
        public string NmEmpresa { get; set; }
        public string FlManejoInterfaz { get; set; }
        public string FlHabilitado { get; set; }
        public int? CdInterfazExterna { get; set; }
    }
}
