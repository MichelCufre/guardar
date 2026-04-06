using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class CompatibilidadContenedor
    {
        public int Empresa { get; set; }
        public string CodigoCliente { get; set; }
        public string NumeroPedido { get; set; }
        public string Direccion { get; set; }
        public string CompartContenedorEntrega { get; set; }
    }
}
