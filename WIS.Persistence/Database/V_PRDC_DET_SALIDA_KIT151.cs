namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRDC_DET_SALIDA_KIT151")]
    public partial class V_PRDC_DET_SALIDA_KIT151
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string CD_PRDC_DEFINICION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public int CD_EMPRESA { get; set; }

        public decimal CD_FAIXA { get; set; }

        public decimal? QT_COMPLETA { get; set; }

        public decimal? QT_FORMULA { get; set; }

        public decimal? QT_LINEA { get; set; }

        public decimal? QT_PRODUCIDO { get; set; }

        public decimal? QT_FORMULA_FORM { get; set; }

        public decimal? QT_LINEA_FORM { get; set; }

        public decimal? QT_PRODUCIDO_FORM { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }
    }
}
