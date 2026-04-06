namespace WIS.Domain.Picking.Dtos
{
    public class AnularPickingPedidoPendiente
    {
        public string Pedido { get; set; }
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public string Cliente { get; set; }
        public int Preparacion { get; set; }
        public int Empresa { get; set; }
        public string EstadoPicking { get; set; }
        public long NuTransaccion { get; set; }
        public int NroAnulacionPreparacion { get; set; }
        public string EstadoAnulacion { get; set; }
    }
}
