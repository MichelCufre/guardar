using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class AnulacionPendienteCamion
    {
        public int Empresa { get; set; }
        public int Contenedor { get; set; }
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public long Carga { get; set; }
        public int Camion { get; set; }
        public int Preparacion { get; set; }
        public string Producto { get; set; }
    }
}
