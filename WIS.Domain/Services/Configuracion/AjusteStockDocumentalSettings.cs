using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Services.Configuracion
{
    public class AjusteStockDocumentalSettings
    {
        public const string Position = "AjusteStockDocumentalSettings";

        public int CantidadAjustes { get; set; }
        public string MutexId { get; set; } 
        public int? MutexTimeout { get; set; }
    }
}
