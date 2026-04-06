using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Produccion.DTOs
{
    public class IngresosGeneradosApiProduccion
    {
        public string IdIngreso { get; set; }

        public string PedidoGenerado { get; set; }

        public int? PreparacionGenerada { get; set; }
    }
}
