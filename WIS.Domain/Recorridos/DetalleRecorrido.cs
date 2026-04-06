using System;

namespace WIS.Domain.Recorridos
{
    public class DetalleRecorrido
    {
        public string NuOrden { get; set; }
        public string TipoOperacion { get; set; }

        public long Id { get; set; }
        public int IdRecorrido { get; set; }
        public string Ubicacion { get; set; }
        public long? NumeroOrden { get; set; }
        public string ValorOrden { get; set; }
        public long? Transaccion { get; set; }
        public long? TransaccionDelete { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
