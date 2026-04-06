namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_LIMP010DET_IMPRESION")]
    public partial class V_LIMP010DET_IMPRESION
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_IMPRESION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGISTRO { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_DATO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ESTADO { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_DOMINIO_VALOR { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ERROR { get; set; }

        public DateTime? DT_PROCESADO { get; set; }
    }
}
