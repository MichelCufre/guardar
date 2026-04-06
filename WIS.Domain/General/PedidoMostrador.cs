using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
    public class PedidoMostrador
    {
        public int NuContenedor { get; set; }
        public int NuPreparacion { get; set; }
        public string Ubicacion { get; set; }
        public string NuPedido { get; set; }
        public string CodigoCliente { get; set; }
        public int CodigoEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string DescripcionCliente { get; set; }
    }
}
