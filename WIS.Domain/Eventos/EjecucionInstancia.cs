using System;
using System.Collections.Generic;

namespace WIS.Domain.Eventos
{
    public partial class EjecucionInstancia
    {
        public int NumeroInstancia { get; set; }
        public DateTime? FechaUltimaEjecucion { get; set; }
    }
}
