namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_MOTIVO_AJUSTE")]
    public partial class T_MOTIVO_AJUSTE
    {
        [Key]
        [StringLength(3)]
        [Column]
        public string CD_MOTIVO_AJUSTE { get; set; }

        [Required]
        [StringLength(60)]
        [Column]
        public string DS_MOTIVO_AJUSTE { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
