using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("T_GRUPO")]
    public partial class T_GRUPO
    {
        [Key]
        [StringLength(50)]
        [Column]
        public string CD_GRUPO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_GRUPO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        public string FL_DEFAULT { get; set; }
    }
}
