using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_ORT080_ORDEN_TAREA_EQUIPO")]
    public partial class V_ORT080_ORDEN_TAREA_EQUIPO
    {
        public long NU_ORT_ORDEN_TAREA_EQUIPO { get; set; }

        [StringLength(60)]
        public string DS_ORT_ORDEN { get; set; }

        public long NU_ORDEN_TAREA { get; set; }

        public int CD_EQUIPO { get; set; }

        [Required]
        [StringLength(40)]
        public string DS_EQUIPO { get; set; }

        public DateTime DT_DESDE { get; set; }

        public DateTime? DT_HASTA { get; set; }

        [StringLength(200)]
        public string DS_MEMO { get; set; }
    }
}
