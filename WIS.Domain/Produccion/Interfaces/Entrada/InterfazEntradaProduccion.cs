using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Produccion.Interfaces.Entrada
{
    public class InterfazEntradaProduccion
    {
        public string IdProcesado { get; set; }
        public long NumeroEjecucion { get; set; }
        public string NumeroRegistro { get; set; }
        public DateTime? FechaAgregado { get; set; }
        public string NumeroIngreso { get; set; }
    }
}
