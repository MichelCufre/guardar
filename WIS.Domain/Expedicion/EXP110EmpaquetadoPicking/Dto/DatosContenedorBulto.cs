using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class DatosContenedorBulto
    {
        public int NumeroContenedor { get; set; }
        public int NumeroPreparacion { get; set; }
        public string DescripcionCliente { get; set; }
        public int CantidadBultos { get; set; }
        public string CodigoCliente { get; set; }
        public string NumeroPedido { get; set; }
        public int Empresa { get; set; }
        public string DescripcionMemo { get; set; }
        public string CodigoClaseEmpaque { get; set; }

    }
}
