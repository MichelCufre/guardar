using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class ControlDeCalidad
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Sigla { get; set; }
        public bool EsBloqueante { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
