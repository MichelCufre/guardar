using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Interfaces
{
    public class InterfazExterna
    {
        public int CodigoInterfazExterna { get; set; }

        public string Descripcion { get; set; }

        public string TipoArchivo { get; set; }

        public int CodigoInterfaz { get; set; }

        public string ReconoPrefijo { get; set; }

        public string ReconoPostfijo { get; set; }

        public string ReconoExtension { get; set; }

        public string ReconoContenido { get; set; }

        public short? NuReconoOrden { get; set; }

        public string Delimitador { get; set; }

        public string NombreProcedimiento { get; set; }

        public DateTime? FechaALta { get; set; }

        public decimal? ComienzoProceso { get; set; }

        public string DelimitadorSegmento { get; set; }

        public string IdSecuencia { get; set; }

        public string ProcExtraeSecuencia { get; set; }

        public string ReProcesable { get; set; }

        public string Endpoint { get; set; }

        public string EndpointReprocess { get; set; }

        public string ParametroDeHabilitacion { get; set; }

        public Interfaz Interfaz{ get; set; }
        public virtual ICollection<InterfazEjecucion> Interfaces { get; set; }
    }
}
