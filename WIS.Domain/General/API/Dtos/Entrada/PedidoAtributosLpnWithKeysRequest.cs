namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class PedidoAtributosLpnWithKeysRequest : DetallePedidoLpnAtributosRequest
    {
        public string NroPedido { get; set; }

        public string CodigoAgente { get; set; }

        public string TipoAgente { get; set; }

        public string CodigoProducto { get; set; }

        public string Identificador { get; set; }

        public string IdLpnExterno { get; set; }

        public string TipoLpn { get; set; }

        public string IdConfiguracion { get; set; }
    }
}
