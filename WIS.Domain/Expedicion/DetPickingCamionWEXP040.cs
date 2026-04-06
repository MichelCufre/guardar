using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class DetPickingCamionWEXP040
    {
           
        public string ID_AGRUPACION { get; set; }
        public int CodigoCamion { get; set; }
        public long NumeroCarga { get; set; }
        public int NumeroPreparacion { get; set; }
        public int CodigoEmpresa { get; set; }
        public string NumeroPedido { get; set; }
        public string CodigoCliente { get; set; }
        public int? NumeroContenedor { get; set; }
        public string PermiteFactSinPrecinto { get; set; }
        public string Agrupacion { get; set; }
        public string EmpaquetaContenedor { get; set; }
    }
}
