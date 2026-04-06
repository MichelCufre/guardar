using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking.Dtos
{
    public class ValidacionDetallePedidoLpnResultado
    {
        public string Message { get; set; }
        public List<string> Datos { get; set; }

        public ValidacionDetallePedidoLpnResultado(string message, List<string> datos)
        {
            this.Message = message;
            this.Datos = datos;
        }
    }
}
