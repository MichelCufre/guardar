using System;

namespace WIS.Domain.Recepcion
{
	public class EstacionDeClasificacion
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaAdicion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Predio { get; set; }
        public string Ubicacion { get; set; }
    }
}
