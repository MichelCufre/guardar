using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class AplicacionCampo
    {
        public string CodigoAplicacion { get; set; }//CD_APLICACION
        public string CodigoCampo { get; set; }//CD_CAMPO
        public string Descripcion { get; set; }//DS_CAMPO
        public string FlagCodigoMultidato { get; set; }//FL_CODIGO_MULTIDATO
    }
}
