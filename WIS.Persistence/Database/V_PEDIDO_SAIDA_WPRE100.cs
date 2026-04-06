namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PEDIDO_SAIDA_WPRE100")]
    public partial class V_PEDIDO_SAIDA_WPRE100
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

        [StringLength(400)]
        [Column]
        public string DS_ENDERECO { get; set; }

        public int? CD_ROTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ROTA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        public DateTime? DT_LIBERAR_DESDE { get; set; }

        public DateTime? DT_LIBERAR_HASTA { get; set; }

        [StringLength(10)]
        [Column]
        public string DT_ENTREGA { get; set; }

        [StringLength(5)]
        [Column]
        public string HR_ENTREGA { get; set; }

        public DateTime? DT_EMITIDO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO_1 { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANUAL { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public short? NU_ORDEN_LIBERACION { get; set; }

        public int? NU_ULT_PREPARACION { get; set; }

        public DateTime? DT_ULT_PREPARACION { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO { get; set; }

        public int? NU_PREPARACION_MANUAL { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_ORIGEN { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        public long? NU_INTERFAZ_FACTURACION { get; set; }

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

        [Required]
        [StringLength(6)]
        [Column]
        public string TP_PEDIDO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_EXPEDICION { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(60)]
        [Column]
        public string NM_EXPEDICION { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        public decimal? QT_PENDIENTE_LIB { get; set; }

        public decimal? QT_PENDIENTE_PREP { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public decimal? QT_FACTURADO { get; set; }

        public decimal? QT_EXPEDIDA { get; set; }

        public decimal? QT_PEND_EXPEDIR { get; set; }
    }
}
