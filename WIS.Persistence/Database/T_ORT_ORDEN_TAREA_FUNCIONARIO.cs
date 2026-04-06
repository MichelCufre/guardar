namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ORT_ORDEN_TAREA_FUNCIONARIO")]
    public partial class T_ORT_ORDEN_TAREA_FUNCIONARIO
    {
        public long NU_ORT_ORDEN_TAREA_FUNC { get; set; }

        public int CD_FUNCIONARIO { get; set; }
        
        [StringLength(200)]
        public string DS_MEMO { get; set; }
        
        public long NU_ORDEN_TAREA { get; set; }

        public DateTime DT_DESDE { get; set; }

        public DateTime? DT_HASTA { get; set; }

        public virtual T_ORT_ORDEN_TAREA T_ORT_ORDEN_TAREA { get; set; }
    }
}
