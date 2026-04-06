using System.Collections.Generic;

namespace WIS.WebhookClient.Models
{
    public class PedidosAnuladosRequest
    {
        public List<PedidoAnuladoRequest> PedidosAnulados { get; set; }

        public PedidosAnuladosRequest()
        {
            PedidosAnulados = new List<PedidoAnuladoRequest>();
        }
    }
}
