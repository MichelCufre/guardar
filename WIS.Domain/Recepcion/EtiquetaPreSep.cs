using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class EtiquetaPreSep
    {
        public decimal? QT_PRODUTO { get; set; }
        public string NU_PREDIO { get; set; }
        public string NU_EXTERNO_ETIQUETA { get; set; }
        public int NU_ETIQUETA_LOTE { get; set; }
        public int NU_AGENDA { get; set; }
        public short? MIN_SITUACAO { get; set; }
        public short? MAX_SITUACAO { get; set; }
        public string ID_CTRL_ACEPTADO { get; set; }
        public string DS_CLIENTE { get; set; }
        public short? CD_SITUACAO { get; set; }
        public string CD_ENDERECO { get; set; }
        public int? CD_EMPRESA { get; set; }
        public string CD_CLIENTE { get; set; }
    }
}
