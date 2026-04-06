using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class EstanPedidoSalidaDet
    {
        public string IdProcesado { get; set; }
        public long Id { get; set; }
        public string Registro{ get; set; }
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public string Producto{ get; set; }
        public string NuIdentificador{ get; set; }
        public string Agrupacion { get; set; }
        public string QtPedido { get; set; }
        public string AddRow{ get; set; }
        public string DescMemo { get; set; }
        public string Empresa { get; set; }
        public DateTime? AddRowInterfaz { get; set; }
        public string VlPorcentajeTolerancia { get; set; }
        public string DtGenerico1 { get; set; }
        public string NuGenerico1 { get; set; }
        public string VlGenerico1{ get; set; }
    }
}
