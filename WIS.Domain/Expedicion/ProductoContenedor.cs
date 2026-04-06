using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class ProductoContenedor
    {
        public string Ubicacion { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CodigoProducto { get; set; }
        public decimal CodigoFaixa { get; set; }
        public string Lote { get; set; }
        public decimal? CantidadPreparada { get; set; }
        
    }
}
