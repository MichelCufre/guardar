namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_RECEPCION_REFERENCIA_TIPO")]
    public partial class T_RECEPCION_REFERENCIA_TIPO
    {
        [Key]
        [StringLength(6)]
        [Column]
        public string TP_REFERENCIA { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_REFERENCIA { get; set; }
    }
}
