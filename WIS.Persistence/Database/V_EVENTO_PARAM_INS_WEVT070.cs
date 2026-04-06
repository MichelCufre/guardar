namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EVENTO_PARAM_INS_WEVT040")]
    public partial class V_EVENTO_PARAM_INS_WEVT040
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string CD_EVENTO_PARAMETRO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_EVENTO_PARAMETRO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_REQUERIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? NU_EVENTO_INSTANCIA { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_PARAMETRO { get; set; }

        [StringLength(30)]
        [Column]
        public string TP_PARAMETRO { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_EXPRESION_REGULAR { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(60)]
        public string TP_NOTIFICACION { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO { get; set; }
    }
}
