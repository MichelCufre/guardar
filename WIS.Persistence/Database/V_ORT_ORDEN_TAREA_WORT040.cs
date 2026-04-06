using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_ORT_ORDEN_TAREA_WORT040")]
    public partial class V_ORT_ORDEN_TAREA_WORT040
    {
        public long NU_ORDEN_TAREA { get; set; }

        public int NU_ORT_ORDEN { get; set; }

        [StringLength(60)]
        public string DS_ORT_ORDEN { get; set; }

        [Required]
        [StringLength(10)]
        public string CD_TAREA { get; set; }

        public int CD_EMPRESA { get; set; }

        public int? CD_FUNCIONARIO_ADDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        public string FL_RESUELTA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(60)]
        public string DS_TAREA { get; set; }

        [Required]
        [StringLength(6)]
        public string TP_TAREA { get; set; }

        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }

    }
}
