using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.OrdenTarea
{
    public class TareaAmigable
    {

        public int NU_ORT_ORDEN { get; set; }
        public string TP_ORDEN { get; set; }
        public string DS_ORDEN { get; set; }
        public string DS_TAREA { get; set; }
        public string NM_EMPRESA { get; set; }
        public string DS_SIGNIFICADO { get; set; }
        public string CD_TAREA { get; set; }
        public int CD_EMPRESA { get; set; }
        public int CD_FUNCIONARIO { get; set; }
        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
        public string DS_MEMO { get; set; }
        public string NU_COMPONENTE { get; set; }
    }
}
