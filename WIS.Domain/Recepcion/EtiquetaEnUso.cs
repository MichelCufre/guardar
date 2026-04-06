using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Impresiones;

namespace WIS.Domain.Recepcion
{
    public class EtiquetaEnUso
    {
        public int Numero { get; set; }                         //NU_ETIQUETA_LOTE

        public string NumeroExterno { get; set; }               //NU_EXTERNO_ETIQUETA

        public string TipoEtiqueta { get; set; }                //TP_ETIQUETA

    }
}
