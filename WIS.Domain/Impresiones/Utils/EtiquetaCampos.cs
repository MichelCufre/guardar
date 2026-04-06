using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Impresiones.Utils
{
    public class EtiquetaCampos
    {
        public string ContenidoEtiquetaWis { get; set; }
        public string ContenidoEtiquetaCampo { get; set; }
        public string ContenidoEtiquetaLargo { get; set; }
        public string ContenidoEtiquetaTexto { get; set; }

        public List<string> Campos { get; set; }
        public List<string> Largos { get; set; }
        public List<string> Textos { get; set; }


        public EtiquetaCampos()
        {
            this.ContenidoEtiquetaWis = string.Empty;
            this.ContenidoEtiquetaCampo = string.Empty;
            this.ContenidoEtiquetaLargo = string.Empty;
            this.ContenidoEtiquetaTexto = string.Empty;

            this.Campos = new List<string>();
            this.Largos = new List<string>();
            this.Textos = new List<string>();
        }

    }
}
