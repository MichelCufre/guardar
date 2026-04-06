using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class ProductoFamilia
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime Alta { get; set; }
        public DateTime Modificacion { get; set; }
    }
}
