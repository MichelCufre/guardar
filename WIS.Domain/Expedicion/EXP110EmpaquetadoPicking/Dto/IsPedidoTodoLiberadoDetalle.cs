using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class IsPedidoTodoLiberadoDetalle
    {
        public decimal CantidadLiberada { get; set; }
        public decimal CantidadPedido { get; set; }
        public decimal CantidadAnulado { get; set; }

    }
}
