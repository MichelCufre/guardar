using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class Moneda
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Simbolo { get; set; }
        public DateTime? Alta { get; set; }
        public DateTime? Modificacion { get; set; }
    }
}
