using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{
    [Table("T_LPN_BARRAS")]
    public partial class T_LPN_BARRAS
    {
        [Key]
        [Column(Order = 0)]
        public int ID_LPN_BARRAS { get; set; }

        [Column(Order = 1)]
        public long NU_LPN { get; set; }

        [Required]
        [StringLength(100)]
        public string CD_BARRAS { get; set; }

        public short? NU_ORDEN { get; set; }

        [Required]
        [StringLength(2)]
        public string TP_BARRAS { get; set; }
    }
}
