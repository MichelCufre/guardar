namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRDC_PRODUCIDO_BB_KIT151")]
    public partial class V_PRDC_PRODUCIDO_BB_KIT151
    {
        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_PRDC_DEFINICION { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        public decimal? QT_PRODUCIDO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }
    }
}
