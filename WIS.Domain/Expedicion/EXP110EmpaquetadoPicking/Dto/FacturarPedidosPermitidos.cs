using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class FacturarPedidosPermitidos
    {
        public int Empresa { get; set; }
        public string Cliente { get; set; }
        public string Pedido { get; set; }
        public bool EmpaquetaContenedor { get; set; }
        public bool PermiteFacturacionParcial { get; set; }
    }
}
