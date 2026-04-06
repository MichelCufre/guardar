namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_MONEDA")]
    public partial class T_MONEDA
    {
        [Key]
        [StringLength(15)]
        [Column]
        public string CD_MONEDA { get; set; }

        [StringLength(80)]
        [Column]
        public string DS_MONEDA { get; set; }

        [StringLength(4)]
        [Column]
        public string DS_SIMBOLO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
