namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE110_DET_PEDIDO_SALIDA")]
    public partial class V_PRE110_DET_PEDIDO_SALIDA
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        public decimal? QT_PENDIENTE { get; set; }

        public decimal? AUXQT_ANULADO { get; set; }

        public string AUXDS_MOTIVO { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? NU_PREPARACION_MANUAL { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_PEDIDO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_TIPO_PEDIDO { get; set; }
        public decimal? QT_PENDIENTE_LPN { get; set; }

        public int? NU_ULT_PREPARACION { get; set; }

        public int? CD_TRANSPORTADORA { get; set; }

        [StringLength(100)]
        public string DS_TRANSPORTADORA { get; set; }

        public int? CD_ROTA { get; set; }

        [StringLength(30)]
        public string DS_ROTA { get; set; }

    }
}
