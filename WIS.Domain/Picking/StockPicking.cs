using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class StockPicking
    {
        public string Ubicacion { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime? Vencimiento { get; set; }
        public int IdRow { get; set; }
    }
}
