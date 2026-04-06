using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class TempPedidoMostrador
    {
        public int NumeroPreparacion { get; set; }
        public int NumeroContenedor { get; set; }
        public string NumeroPedido { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CodigoCliente { get; set; }
        public long NumeroCarga { get; set; }
        public Nullable<int> CodigoCamionFacturado{ get; set; }
        public string CodigoUbicacion { get; set; }
	}
}
