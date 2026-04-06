using System;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoCaracteristica
    {
        public decimal Id { get; set; }
        public int IdAutomatismo { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }
        public string ValorAuxiliar { get; set; }
        public long? NumeroAuxiliar { get; set; }
        public decimal? CantidadAuxiliar { get; set; }
        public bool FlagAuxiliar { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? Transaccion { get; set; }
    }
}
