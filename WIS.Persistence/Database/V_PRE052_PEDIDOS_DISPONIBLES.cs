namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_PRE052_PEDIDOS_DISPONIBLES")]
    public partial class V_PRE052_PEDIDOS_DISPONIBLES
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [Required]
        [StringLength(6)]
        [Column]
        public string TP_PEDIDO { get; set; }

        public int? CD_ROTA { get; set; }

        public int? NU_PREPARACION_MANUAL { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        public DateTime? DT_LIBERAR_DESDE { get; set; }

        public DateTime? DT_LIBERAR_HASTA { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        public DateTime? DT_EMITIDO { get; set; }

        public short? NU_ORDEN_LIBERACION { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO_1 { get; set; }

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

		[StringLength(400)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_EXPEDICION { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
    }
}
