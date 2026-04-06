using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.OrdenTarea
{
    public class OrdenTareaEquipo
    {
        public long NuOrtOrdenTareaEquipo { get; set; }
        public int CdEquipo { get; set; }
        public long NuOrdenTarea { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string DescripcionMemo { get; set; }
    }
}