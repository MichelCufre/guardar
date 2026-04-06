namespace WIS.Domain.Picking
{
    public class PedidoContenedor
    {
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public int? Ruta { get; set; }
        public string Predio { get; set; }
        public string ComparteContenedorEntrega { get; set; }
        public string ComparteContenedorPicking { get; set; }
        public int Preparacion { get; set; }
        public int Contenedor { get; set; }
        public short EstadoContenedor { get; set; }
    }
}
