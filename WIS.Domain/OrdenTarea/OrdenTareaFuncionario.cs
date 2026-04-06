using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.OrdenTarea
{
    public class OrdenTareaFuncionario
    {
        public long NuOrtOrdenTareaFuncionario { get; set; }
        public int CodigoFuncionario { get; set; }
        public string DescripcionMemo { get; set; }
        public long NumeroOrdenTarea { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}
