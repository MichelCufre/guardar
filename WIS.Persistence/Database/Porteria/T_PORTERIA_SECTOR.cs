namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("T_PORTERIA_SECTOR")]
    public partial class T_PORTERIA_SECTOR
    {
        [Key]
        [StringLength(30)]
        [Column]
        public string CD_SECTOR { get; set; }

        [StringLength(300)]
        [Column]
        public string DS_SECTOR { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public short? CD_PORTA { get; set; }
    }
}
