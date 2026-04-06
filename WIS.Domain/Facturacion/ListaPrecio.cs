using System;

namespace WIS.Domain.Facturacion
{
	public class ListaPrecio
    {
        public int Id { get; set; }
        public string Descripcion{ get; set; }
        public DateTime? FechaIngresado { get; set; }
        public DateTime? FechaActualizacion{ get; set; }
        public string IdMoneda{ get; set; }
        public long? NumeroTransaccion { get; set; }
        public long? NumeroTransaccionDelete { get; set; }
    }
}
