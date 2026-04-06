namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class PedidoAtributosWithKeysRequest : DetallePedidoAtributosRequest
    {
        public string NroPedido { get; set; }

        public string CodigoAgente { get; set; }

        public string TipoAgente { get; set; }

        public string CodigoProducto { get; set; }

        public string Identificador { get; set; }

        public string IdConfiguracion { get; set; }
    }
}
