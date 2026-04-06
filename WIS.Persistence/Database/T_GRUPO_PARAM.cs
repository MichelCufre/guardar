using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("T_GRUPO_PARAM")]
    public partial class T_GRUPO_PARAM
    {
        [Key]
        [Column]
        public int NU_PARAM { get; set; }

        [StringLength(50)]
        [Column]
        public string NM_PARAM { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_PARAM { get; set; }

        [Column]
        public int? NU_ORDEN { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_PARAM_DEFAULT { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_PARAM { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}