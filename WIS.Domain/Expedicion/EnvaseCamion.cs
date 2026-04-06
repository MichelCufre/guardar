using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.Expedicion
{
    public class EnvaseCamion
    {
        public int Contenedor { get; set; }
        public int Preparacion { get; set; }
        public string IdExterno { get; set; }
        public string Tipo { get; set; }
        public string TipoAgente { get; set; }
        public string CodigoAgente { get; set; }
        public int Empresa { get; set; }
    }
}
