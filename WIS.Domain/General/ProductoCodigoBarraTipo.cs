using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class ProductoCodigoBarraTipo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime? FechaInsercion { get; set; }
    }
}