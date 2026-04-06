using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class Clase
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public string IdSuperClase { get; set; }
        public DateTime FechaInsercion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
