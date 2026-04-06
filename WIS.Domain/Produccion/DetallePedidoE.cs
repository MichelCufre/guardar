namespace WIS.Domain.Produccion
{
    public class DetallePedidoE
    {
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public decimal cantidad { get; set; }
        public string Semiacabado { get; set; }
        public string Predio { get;  set; }
        public string Consumible { get; set; }
    }
}
