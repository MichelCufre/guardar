namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_REG300_PARAMETROS")]
    public partial class V_REG300_PARAMETROS
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
        public string VL_PARAM { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_PARAM { get; set; }
    }
}
