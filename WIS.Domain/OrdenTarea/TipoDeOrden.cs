using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.OrdenTarea
{
    public class TipoDeOrden
    {
        public string TP_ORDEN { get; set; }
        public string DS_ORDEN { get; set; }
        public int? HR_INICIAL_DIA_SEMANA { get; set; }
        public decimal? HR_FINAL_DIA_SEMANA { get; set; }
        public decimal? HR_INICIAL_SABADO { get; set; }
        public decimal? HR_FINAL_SABADO { get; set; }
    }
}
