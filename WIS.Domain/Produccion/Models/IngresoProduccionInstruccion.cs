namespace WIS.Domain.Produccion.Models
{
	public class IngresoProduccionInstruccion
	{
		public int? Id { get; set; }

		public string IdIngreso { get; set; }

		public byte[] ValorInstruccion { get; set; }

		public string TipoValor { get; set; }
	}
}
