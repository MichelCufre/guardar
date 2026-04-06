using System;

namespace WIS.Domain.General
{
	public class SugerenciaAlmacenamientoDetalle
	{
		public int Empresa { get; set; }
		public string Producto { get; set; }
		public decimal Faixa { get; set; }
		public string Lote { get; set; }
		public decimal CantidadSeparar { get; set; }
		public DateTime? Vencimiento { get; set; }
		public decimal CantidadAuditada { get; set; }
		public decimal CantidadClasificada { get; set; }
        public string UbicacionSugerida { get; set; }
        public string Estado { get; set; }
    }
}
