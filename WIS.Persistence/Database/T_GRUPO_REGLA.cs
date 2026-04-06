using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("T_GRUPO_REGLA")]
    public partial class T_GRUPO_REGLA
    {
        [Key]
        [Column]
        public long NU_GRUPO_REGLA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_REGLA { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_GRUPO { get; set; }

        [Column]
        public int NU_ORDEN { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
