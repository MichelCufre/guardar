using System;

namespace WIS.Domain.Produccion.Models
{
	public class IngresoProduccionDetalleTeorico
	{
		public long? Id { get; set; }

		public string IdIngresoProduccion { get; set; }

		public string Tipo { get; set; }

		public string Producto { get; set; }

		public decimal? Faixa { get; set; }

		public int? Empresa { get; set; }

		public decimal? CantidadTeorica { get; set; }

		public decimal? CantidadPedidoGenerada { get; set; }

		public decimal? CantidadAbastecido { get; set; }

		public decimal? CantidadConsumida { get; set; }

		public string Identificador { get; set; }

		public long? NumeroTransaccion { get; set; }

		public DateTime? FechaAlta { get; set; }

		public string Anexo1 { get; set; }

		public string Anexo2 { get; set; }

		public string Anexo3 { get; set; }

		public string Anexo4 { get; set; }
	}
}
