using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class Situacion
    {
        public short Id { get; set; }
        public string Descripcion { get; set; }
        public bool Interno { get; set; }
        public DateTime FechaInsercion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
