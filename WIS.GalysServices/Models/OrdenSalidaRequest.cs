using System.Collections.Generic;

namespace WIS.GalysServices.Models
{
    public class OrdenSalidaRequest
    {
        public string codAlmacen { get; set; }
        public string numeroPedidoCliente { get; set; }
        public string codDestinatario { get; set; }
        public string denomDestinatario { get; set; }
        public int prioridad { get; set; }
        public string fechaServicio { get; set; }
        public string tipoPedido { get; set; }
        public List<PedidoClienteLineaRequest> listaLineas { get; set; }
    }

    public class PedidoClienteLineaRequest
    {
        public int lineaSalida { get; set; }
        public string codArticulo { get; set; }
        public string lote { get; set; }
        public int cantidadSolicitada { get; set; }
    }
}
