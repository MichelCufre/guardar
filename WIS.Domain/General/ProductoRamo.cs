using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class ProductoRamo
    {
        public short Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime? Alta { get; set; }
        public DateTime? Modificacion { get; set; }
    }
}
