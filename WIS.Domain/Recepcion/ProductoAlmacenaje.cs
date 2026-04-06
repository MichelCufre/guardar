using System;

namespace WIS.Domain.Recepcion
{
	public class ProductoAlmacenaje
	{
        public int Etiqueta { get; set; }
        public int UserId { get; set; }
        public int Empresa { get; set; }
		public string Codigo { get; set; }
		public string Lote { get; set; }
		public decimal Faixa { get; set; }
		public decimal CantidadSeparar { get; set; }
		public decimal CantidadAuditada { get; set; }
		public decimal CantidadClasificada { get; set; }
		public decimal? Volumen { get; set; }
		public decimal? Alto { get; set; }
		public decimal? Ancho { get; set; }
		public decimal? Profundidad { get; set; }
		public DateTime? Vencimiento { get; set; }
		public decimal? Peso { get; set; }
		public decimal? UnidadesPorPallet { get; set; }
		public string Clase { get; set; }
        public decimal UnidadBulto { get;  set; }
        public string UbicacionDestino{ get; set; }
    }
}
