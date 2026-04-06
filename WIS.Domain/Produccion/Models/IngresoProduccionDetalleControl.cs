namespace WIS.Domain.Produccion.Models
{
	public class IngresoProduccionDetalleControl
	{
		public long? Id { get; set; }

		public string IdIngreso { get; set; }

		public int? IdControl { get; set; }

		public int? IdControlPendiente { get; set; }
	}
}
