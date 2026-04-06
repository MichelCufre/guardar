namespace WIS.Domain.Services.Configuracion
{
	public class MaxItemsSettings
    {
        public const string Position = "MaxItemsSettings";

        public int Producto { get; set; }
        public int Pedido { get; set; }
        public int CodigoBarras { get; set; }
        public int ProductoProveedor { get; set; }
        public int Agente { get; set; }
        public int ReferenciaRecepcion { get; set; }
        public int AnulacionReferenciaRecepcion { get; set; }
        public int ModificarDetalleReferencia { get; set; }
        public int Empresa { get; set; }
        public int Agenda { get; set; }
        public int PageSizeStock { get; set; }
        public int ModificarPedido { get; set; }
        public int Egreso { get; set; }
        public int Lpn { get; set; }
        public int AjustesStock { get; set; }
        public int TransferenciaStock { get; set; }
        public int CrossDockingUnaFase { get; set; }
        public int Picking { get; set; }
        public int AnularPickingPedidoPendiente { get; set; }
        public int Produccion { get; set; }
        public int ProducirProduccion { get; set; }
        public int ConsumirProduccion { get; set; }
        public int ImportarUbicacion { get; set; }
        public int ImportarDetallesRecorrido { get; set; }
        public int ControlDeCalidad { get; set; }
        public int Facturas { get; set; }
        public int UbicacionesPicking { get; set; }

    }
}
