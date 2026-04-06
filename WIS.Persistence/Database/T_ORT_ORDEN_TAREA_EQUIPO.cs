using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_ORT_ORDEN_TAREA_EQUIPO")]
    public partial class T_ORT_ORDEN_TAREA_EQUIPO
    {

        public long NU_ORT_ORDEN_TAREA_EQUIPO { get; set; }

        public long NU_ORDEN_TAREA { get; set; }

        public int CD_EQUIPO { get; set; }

        public DateTime DT_DESDE { get; set; }

        public DateTime? DT_HASTA { get; set; }

        [StringLength(200)]
        public string DS_MEMO { get; set; }

        public virtual T_ORT_ORDEN_TAREA T_ORT_ORDEN_TAREA { get; set; }

    }
}
