using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.OrdenTarea
{
    public class OrdenTareaObjeto
    {
        public long NuTarea { get; set; }
        public int NuOrden { get; set; }
        public string CdTarea { get; set; }
        public int Empresa { get; set; }
        public int? CdFuncionarioAddrow { get; set; }
        public DateTime? DtAddrow { get; set; }
        public DateTime? DtUpdrow { get; set; }
        public string Resuelta { get; set; }
    }
}
