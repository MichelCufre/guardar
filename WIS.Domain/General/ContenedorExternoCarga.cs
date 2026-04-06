using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class ContenedorExternoCarga
    {
        public string IdExternoContenedor { get; set; }
        public string TipoContenedor { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public long Carga { get; set; }
    }

}
