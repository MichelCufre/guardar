using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class EjecucionesPendientesResponse
    {
        public List<EjecucionPendienteResponse> Ejecuciones { get; set; }
        public EjecucionesPendientesResponse()
        {
            Ejecuciones = new List<EjecucionPendienteResponse>();
        }
    }

    public class EjecucionPendienteResponse
    {
        public long NumeroInterfazEjecucion { get; set; }

        public int CodigoInterfazExterna { get; set; }

    }
}
