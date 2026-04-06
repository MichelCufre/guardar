using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class SuperClase
    {
        public SuperClase()
        {
            Clases = new List<Clase>();
        }
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInsercion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public List<Clase> Clases { get; set; }
    }
}
