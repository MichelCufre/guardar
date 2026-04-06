using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("T_GRUPO_REGLA_PARAM")]
    public partial class T_GRUPO_REGLA_PARAM
    {
        [Key]
        [Column]
        public long NU_GRUPO_REGLA_PARAM { get; set; }

        [Column]
        public long NU_GRUPO_REGLA { get; set; }

        [Column]
        public int NU_PARAM { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_PARAM { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
