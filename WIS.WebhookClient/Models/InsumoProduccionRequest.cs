namespace WIS.WebhookClient.Models
{
	public class InsumoProduccionRequest
	{
		public string CodigoProducto { get; set; }

		public string Identificador { get; set; }

		public decimal? CantidadTeorica { get; set; }

		public decimal? CantidadConsumida { get; set; }

        public string CodigoMotivo { get; set; }
    }
}
