using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.Liberacion
{
    public class CondicionLiberacion
    {
        public string Condicion { get; set; }
        public string Descripcion { get; set; }
        public bool MostrarMarcada { get; set; }

    }
}
