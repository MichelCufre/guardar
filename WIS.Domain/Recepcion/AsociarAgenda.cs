using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class AsociarAgenda : IEquatable<AsociarAgenda>
    {
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public int IdFactura { get; set; }
        public int? IdAgenda { get; set; }

        public virtual bool Equals(AsociarAgenda other)
        {
            return this.IdFactura == other.IdFactura && this.Empresa == other.Empresa && this.Cliente == other.Cliente && this.IdAgenda == other.IdAgenda;
        }

        public override int GetHashCode() => (IdFactura, Empresa, Cliente, IdAgenda).GetHashCode();

    }
}
