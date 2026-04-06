using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtoReporteControlCambioLpn
    {
        public int Camion { get; set; }
        public long NroLpn { get; set; }
        public string NombreAtributo { get; set; }
        public string ValorAtributo { get; set; }
    }
}
