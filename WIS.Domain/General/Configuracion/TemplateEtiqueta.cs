using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General.Configuracion
{
    public class TemplateEtiqueta
    {
        public string estilo { get; set; }
        public string lenguaje{ get; set; }
        public string preCuerpo { get; set; }
        public string cuerpo { get; set; }
        public string postCuerpo { get; set; }
        public decimal? altura { get; set; }
        public decimal? largura{ get; set; }
    }
}
