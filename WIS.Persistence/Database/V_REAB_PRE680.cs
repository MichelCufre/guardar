namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REAB_PRE680")]
    public partial class V_REAB_PRE680
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_PREDIO_NECESIDAD { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public decimal QT_UND_BULTO { get; set; }

        public decimal? QT_UNIDADES { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public decimal? QT_NECESIDAD { get; set; }

        public decimal? QT_NECESIDAD_FINAL { get; set; }

        public decimal? QT_PEDIDOS_REAB { get; set; }

        public decimal? QT_RESERVA { get; set; }

        public decimal? QT_STOCK_CONSIDERADO { get; set; }

        public decimal? QT_TRANSITO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX1 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED1 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED1 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX2 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED2 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED2 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX3 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED3 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED3 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX4 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED4 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED4 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX5 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED5 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED5 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX6 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED6 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED6 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX7 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED7 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED7 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX8 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED8 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED8 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX9 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED9 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED9 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO_AUX10 { get; set; }

        public decimal? QT_DISOPNIBLE_PRED10 { get; set; }

        public decimal? QT_PEDIDOS_REAB_PRED10 { get; set; }
    }
}
