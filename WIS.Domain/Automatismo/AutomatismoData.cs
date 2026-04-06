using System;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoData
    {
        public int Id { get; set; }

        public int IdAutomatismoEjecucion { get; set; }

        public string RequestData { get; set; }

        public string ResponseData { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? Transaccion { get; set; }

        public virtual AutomatismoEjecucion AutomatismoEjecucion { get; set; }
    }
}
