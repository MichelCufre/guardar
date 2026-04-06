using System;
using WIS.Domain.Automatismo.Interfaces;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoPuesto
    {
        public int Id { get; set; }
        public int IdAutomatismo { get; set; }
        public string Puesto { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? Transaccion { get; set; }
        public IAutomatismo Automatismo { get; set; }
        public string Impresora { get; set; }
    }
}
