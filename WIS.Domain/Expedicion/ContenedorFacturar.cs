using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
  public class ContenedorFacturar
    {
        public string NumeroPedido { get; set; }
        public string CodigoCliente { get; set; }
        public int CodigoEmpresa { get; set; }
        public int NumeroContenedor { get; set; }
        public int NumeroPreparacion { get; set; }
        public long NumeroCarga { get; set; }
        public int CodigoCamion { get; set; }
	}
}
