using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class ProductoContenedorPuerta
    {
        public int CodigoEmpresa { get; set; }
     
        public string CodigoProducto { get; set; }
        
        public decimal CodigoFaixa { get; set; }
       
        public string Lote { get; set; }
     
        public string CodigoCliente { get; set; }
      
        public string NumeroPedido{ get; set; }
     
        public bool EspecificaLote { get; set; }
       
        public string CodigoUbicacion { get; set; }
       
        public int? CodigoCamion { get; set; }
     
        public decimal? CantidadPreparada { get; set; }
    }
}
