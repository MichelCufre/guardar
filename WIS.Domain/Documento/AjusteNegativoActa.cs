using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Documento
{
    public class AjusteNegativoActa
    {
        public string NU_AJUSTE_STOCK { get; set; }
        public string CD_PRODUCTO { get; set; }
        public string NU_IDENTIFICADOR { get; set; }
        public decimal QT_MOVIMIENTO { get; set; }
        public decimal QT_DESAFECTAR_ACTA { get; set; }
    }
}
