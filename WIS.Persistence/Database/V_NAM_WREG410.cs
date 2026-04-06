namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_NAM_WREG410")]
    public partial class V_NAM_WREG410
    {
        [Key]
        [StringLength(20)]
        [Column(Order = 0)]
        public string CD_NAM { get; set; }

        [StringLength(60)]
        [Column(Order = 1)]
        public string DS_NAM { get; set; }
    }
}
