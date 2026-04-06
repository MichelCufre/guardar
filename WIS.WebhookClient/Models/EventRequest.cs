using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class EventRequest
    {
        /// <summary>
        /// Identifica el tipo de evento notificado. 
        /// Para cada tipo de evento se define una propiedad con el mismo nombre que contiene el detalle de la información notificada.
        /// </summary>
        /// <example>confirmacionRecepcion</example>
        [ApiDtoExample("confirmacionRecepcion")]
        public string Id { get; set; }

        public long NumeroInterfazEjecucion { get; set; }

        public AgendaRequest ConfirmacionRecepcion { get; set; }

        public ConfirmacionPedidoRequest ConfirmacionPedido { get; set; }

        public PedidosAnuladosRequest PedidosAnulados { get; set; }

        public AjustesRequest Ajustes { get; set; }

        public ConsultaStockRequest ConsultaStock { get; set; }

        public ConfirmacionMercaderiaPreparadaRequest ConfirmacionMercaderiaPreparada { get; set; }

        public AlmacenamientoRequest Almacenamiento { get; set; }

        public TestRequest Test { get; set; }

        public ConfirmacionProduccionRequest ConfirmacionProduccion { get; set; }
    }
}
