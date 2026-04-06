using System;

namespace WIS.Domain.Recepcion
{
    public class CrossDockingPedidoSelection : IEquatable<CrossDockingPedidoSelection>
    {
        public string Pedido { get; set; }
        public int Empresa { get; set; }
        public string Cliente { get; set; }

        public virtual bool Equals(CrossDockingPedidoSelection other)
        {
            return this.Pedido == other.Pedido && this.Empresa == other.Empresa && this.Cliente == other.Cliente;
        }

        public override int GetHashCode() => (Pedido, Empresa, Cliente).GetHashCode();
    }
}
