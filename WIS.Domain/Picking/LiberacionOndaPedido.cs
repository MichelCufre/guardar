using System.Reflection.Metadata.Ecma335;

namespace WIS.Domain.Picking
{
    public class LiberacionOndaPedido
    {
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public decimal PorcentajeVidaUtil { get; set; }
    }
}
