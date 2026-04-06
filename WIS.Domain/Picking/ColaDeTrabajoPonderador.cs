using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class ColaDeTrabajoPonderador
    {
        public int Numero { get; set; }
        public string Ponderador { get; set; }
        public int? Incremento { get; set; }
        public bool Habilitado { get; set; }
        public DateTime? dtAddrow { get; set; }
        public DateTime? dtUpdrow { get; set; }
    }
}
