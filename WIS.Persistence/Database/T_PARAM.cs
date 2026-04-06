namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PARAM")]
    public partial class T_PARAM
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string CD_APLICACAO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string TP_APLICACAO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string CD_PARAMETRO { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_PARAMETRO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_PARAMETRO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_DISPONIBLE_POR_EMPRESA { get; set; }
    }
}
