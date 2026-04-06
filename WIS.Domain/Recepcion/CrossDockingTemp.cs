using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class CrossDockingTemp
    {
        public int NU_AGENDA { get; set; }
        public string CD_CLIENTE { get; set; }
        public string NU_PEDIDO { get; set; }
        public string CD_PRODUTO { get; set; }
        public decimal CD_FAIXA { get; set; }
        public decimal QT_PRODUTO { get; set; }
        public string NU_IDENTIFICADOR { get; set; }
        public bool ID_ESPECIFICA_IDENTIFICADOR { get; set; }
        public int CD_EMPRESA { get; set; }
        public short Ruta { get; set; }
    }
}
