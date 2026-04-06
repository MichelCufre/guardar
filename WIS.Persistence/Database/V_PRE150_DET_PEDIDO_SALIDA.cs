namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE150_DET_PEDIDO_SALIDA")]
    public partial class V_PRE150_DET_PEDIDO_SALIDA
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
        public string DS_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

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

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        public decimal? QT_PENDIENTE { get; set; }

        public decimal? QT_PENDIENTE_PREP { get; set; }

        [StringLength(1)]
        [Column]
        public string AUXMOTIVO_RECHAZO { get; set; }

        public decimal? QT_EXPEDIDO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        public DateTime? DT_LIBERAR_DESDE { get; set; }

        public DateTime? DT_LIBERAR_HASTA { get; set; }

        public int? NU_ULT_PREPARACION { get; set; }

        public DateTime? DT_ULT_PREPARACION { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANUAL { get; set; }

        public DateTime? DT_EMITIDO { get; set; }

        public int? CD_ROTA { get; set; }

        public decimal? QT_PEDIDO_ORIGINAL { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public int? NU_PREPARACION_MANUAL { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public decimal? QT_FACTURADO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_MEMO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO4 { get; set; }

        public DateTime? DT_GENERICO_1 { get; set; }

        public decimal? NU_GENERICO_1 { get; set; }

        [StringLength(400)]
        [Column]
        public string VL_GENERICO_1 { get; set; }
    }
}
