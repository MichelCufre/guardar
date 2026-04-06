using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class TareaRequest
    {
        public string CodigoExterno { get; set; }
        public string Descripcion { get; set; }
        public string CodigoPuntoEntregaDestino { get; set; }
        public string Prometida { get; set; }
        public string SistemaCreacion { get; set; }
        public string Telefono { get; set; }
        public List<PedidoRequest> Pedidos { get; set; }
    }
}
