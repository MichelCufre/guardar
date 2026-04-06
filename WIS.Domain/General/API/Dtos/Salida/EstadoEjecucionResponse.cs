using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class EstadoEjecucionResponse
    {
        public string Estado { get; set; }
        public int Empresa { get; set; }
        public long NumeroInterfazEjecucion { get; set; }
        public string Mensaje { get; set; }

        public List<EstadoEjecucionErrorResponse> Errores { get; set; }
        public EstadoEjecucionResponse()
        {
            Errores = new List<EstadoEjecucionErrorResponse>();
        }
    }

    public class EstadoEjecucionErrorResponse
    {
        public int NroRegistro { get; set; }
        public int NroError { get; set; }
        public string Error { get; set; }

    }
}
