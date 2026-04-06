using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_TP_EXP")]
    public partial class V_PRE811_PREF_TP_EXP
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_EXPEDICION { get; set; }

        [StringLength(60)]
        public string NM_EXPEDICION { get; set; }

        public int? NU_PREFERENCIA_TP_EXP { get; set; }

    }
}