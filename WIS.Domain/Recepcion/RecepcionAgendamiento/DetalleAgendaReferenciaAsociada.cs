using System;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
	public class DetalleAgendaReferenciaAsociada
    {
        public AgendaDetalle DetalleAgenda { get; set; }
        public ReferenciaRecepcionDetalle DetalleReferencia { get; set; }
        public decimal CantidadAgendada { get; set; }
        public decimal CantidadRecibida { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public long? NumeroTransaccion { get; set; }
        public long? NumeroTransaccionDelete { get; set; }
        public long? NumeroInterfazEjecucion { get; set; }
    }
}
