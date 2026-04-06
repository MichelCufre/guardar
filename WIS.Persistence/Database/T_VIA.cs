namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_VIA")]
    public partial class T_VIA
    {
        [Key]
        [StringLength(4)]
        [Column]
        public string CD_VIA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_VIA { get; set; }

        public short? CD_SITUACAO { get; set; }
    }
}
