using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class ConfiguracionPedido
    {
        public bool ValidarHorasEntreEmisionEntrega { get; set; }
        public bool PedidoDebeSerNumerico { get; set; }
        public bool PermitePedidosAProveedores { get; set; }
    }
}
