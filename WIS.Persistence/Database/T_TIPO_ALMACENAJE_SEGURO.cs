namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TIPO_ALMACENAJE_SEGURO")]
    public partial class T_TIPO_ALMACENAJE_SEGURO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short TP_ALMACENAJE_Y_SEGURO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ALMACENAJE_Y_SEGURO { get; set; }
    }
}
