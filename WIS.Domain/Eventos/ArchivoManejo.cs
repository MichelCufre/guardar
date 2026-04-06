using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Eventos
{
    public class ArchivoManejo
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }
        public List<string> CodigosAnexos { get; set; }
        public List<string> DescripcionAnexos { get; set; }

        public List<string> CodigosCampos { get; set; }
        public List<string> DescripcionCampos { get; set; }
        public virtual List<ArchivoDocumento> TiposDocumentos { get; set; }

    }
}
