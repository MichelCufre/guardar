using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Registro
{
    public class ProductoPallet
    {
        public string CodigoProducto { get; set; }
        public decimal Embalaje { get; set; }
        public int Empresa { get; set; }
        public short CodigoPallet { get; set; }
        public decimal Unidades { get; set; }
        public short Prioridad { get; set; }
    }
}
