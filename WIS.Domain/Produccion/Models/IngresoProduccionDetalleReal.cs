using System;

namespace WIS.Domain.Produccion.Models
{
	public class IngresoProduccionDetalleReal
	{
		public long NuPrdcIngresoReal { get; set; }

		public string NuPrdcIngreso { get; set; }

		public string Producto { get; set; }

		public int? Empresa { get; set; }

		public decimal? Faixa { get; set; }

		public decimal? QtReal { get; set; }

		public decimal? QtRealOriginal { get; set; }

		public decimal? QtNotificado { get; set; }

		public decimal? QtDesafectado { get; set; }

		public decimal? QtMerma { get; set; }

		public string Identificador { get; set; }

		public long? NuTransaccion { get; set; }

		public DateTime? DtAddrow { get; set; }

		public long? NuOrden { get; set; }

		public string DsAnexo1 { get; set; }

		public string DsAnexo2 { get; set; }

		public string DsAnexo3 { get; set; }

		public string DsAnexo4 { get; set; }

		public string Consumible { get; set; }

		public string Referencia { get; set; }

		public string Estado { get; set; }

    }
}
