using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.OrdenTarea
{
    public class TareaPlantilla
    {
        public long Id { get; set; }
        public string CodigoPlantillaTarea { get; set; }
        public string CodigoTarea { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? SecueciaTarea { get; set; }
    }
}
