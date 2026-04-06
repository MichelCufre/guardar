using System;

namespace WIS.WebhookClient.Models
{
	public class ProductoProduccionRequest
	{
		public string CodigoProducto { get; set; }

		public string Identificador { get; set; }
		
		public string Vencimiento { get; set; }

		public decimal? CantidadTeorica { get; set; }

		public decimal? CantidadProducida { get; set; }

        public string ModalidadCalculoLote { get; set; }

		public string CodigoMotivo { get; set; }

		public string Observaciones { get; set; }
	}
}
