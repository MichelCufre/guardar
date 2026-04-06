namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRDC_DET_ENTRADA_KIT151")]
    public partial class V_PRDC_DET_ENTRADA_KIT151
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string CD_COMPONENTE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short NU_PRIORIDAD { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string CD_PRDC_DEFINICION { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public int CD_EMPRESA { get; set; }

        public decimal CD_FAIXA { get; set; }

        public decimal? QT_COMPLETA { get; set; }

        public int? CD_EMPRESA_PEDIDO { get; set; }

        public decimal? QT_FORMULA { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public decimal? QT_LINEA { get; set; }

        public decimal? QT_CONSUMIDO { get; set; }

        public decimal? QT_FORMULA_FORM { get; set; }

        public decimal? QT_PEDIDO_FORM { get; set; }

        public decimal? QT_LIBERADO_FORM { get; set; }

        public decimal? QT_PREPARADO_FORM { get; set; }

        public decimal? QT_LINEA_FORM { get; set; }

        public decimal? QT_CONSUMIDO_FORM { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }
    }
}
