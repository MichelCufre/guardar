using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_ORT_ORDEN_TAREA_FUNC_WORT060")]
    public partial class V_ORT_ORDEN_TAREA_FUNC_WORT060
    {

        public long NU_ORT_ORDEN_TAREA_FUNC { get; set; }

        [StringLength(60)]
        public string DS_ORT_ORDEN { get; set; }

        public long NU_ORDEN_TAREA { get; set; }

        public int CD_FUNCIONARIO { get; set; }

        [Required]
        [StringLength(30)]
        public string NM_FUNCIONARIO { get; set; }

        public DateTime DT_DESDE { get; set; }

        public DateTime? DT_HASTA { get; set; }

        [StringLength(200)]
        public string DS_MEMO { get; set; }

    }
}
