namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_NAM")]
    public partial class T_NAM
    {
        [Key]
        [StringLength(20)]
        [Column]
        public string CD_NAM { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_NAM { get; set; }
    }
}
