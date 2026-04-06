using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class ColaDeTrabajoPonderadorDetalle
    {
        public int Numero { get; set; }
        public string Ponderador { get; set; }
        public string Instancia { get; set; }
        public string Operacion { get; set; }
        public int? NuPonderacion { get; set; }
        public DateTime? dtAddrow { get; set; }
        public DateTime? dtUpdrow { get; set; }
    }
}
