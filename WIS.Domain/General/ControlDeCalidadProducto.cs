using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class ControlDeCalidadProducto
    {
        public int Codigo { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
