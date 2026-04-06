using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Produccion
{
    public class ProductosExpulsable
    {
        public string Ubicacion { get; set; }                   
        public int Empresa { get; set; }                        
        public string Producto { get; set; }                    
        public decimal Faixa { get; set; }                      
        public string Identificador { get; set; }               
        public decimal? Cantidad { get; set; }
        public string UbicacionDestino { get; set; }
        public DateTime? Vencimiento { get; set; }
        public long IdRow { get; set; }
    }
}
