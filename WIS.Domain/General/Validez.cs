using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class Validez
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public short? DiasDuracion { get; set; }
        public short? DiasValidez { get; set; }
        public short? DiasValidezLibracion { get; set; }
    }
}
