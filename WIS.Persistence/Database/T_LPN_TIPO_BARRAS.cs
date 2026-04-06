using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{
    [Table("T_LPN_TIPO_BARRAS")]
    public partial class T_LPN_TIPO_BARRAS
    {
        [Key]
        [StringLength(2)]
        [Column(Order = 0)]
        public string TP_BARRAS { get; set; }

        [StringLength(100)]
        [Column(Order = 1)]
        public string DS_TP_BARRAS { get; set; }
    }
}
