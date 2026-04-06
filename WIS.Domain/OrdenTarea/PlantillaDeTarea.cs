using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class PlantillaDeTarea
    {
        public string CD_PLANTILLA_TAREA { get; set; }
        public string DS_PLANTILLA_TAREA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
