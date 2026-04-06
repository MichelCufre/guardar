namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EXP043_PEDIDOS_PEND_CAMION")]
    public partial class V_EXP043_PEDIDOS_PEND_CAMION
    {
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Required]
        [StringLength(6)]
        [Column]
        public string TP_PEDIDO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_TIPO_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

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
        public string DS_MEMO { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO_1 { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANUAL { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public short? NU_ORDEN_LIBERACION { get; set; }

        public int? NU_ULT_PREPARACION { get; set; }

        public DateTime? DT_ULT_PREPARACION { get; set; }

        public int? NU_PREPARACION_MANUAL { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_ORIGEN { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        public long? NU_INTERFAZ_FACTURACION { get; set; }

        public int? CD_CAMION { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_CAMION { get; set; }
    }
}
