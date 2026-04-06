using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.OrdenTarea
{
    public class HorasPorOrdenFuncionario
    {
        public string TP_ORDEN { get; set; }
        public int NU_ORT_ORDEN { get; set; }
        public decimal? HR_INICIAL_SABADO { get; set; }
        public decimal? HR_INICIAL_DIA_SEMANA { get; set; }
        public decimal? HR_FINAL_SABADO { get; set; }
        public decimal? HR_FINAL_DIA_SEMANA { get; set; }
    }
}
