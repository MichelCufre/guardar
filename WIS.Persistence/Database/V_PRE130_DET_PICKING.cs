namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_PRE130_DET_PICKING")]
    public partial class V_PRE130_DET_PICKING
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        public long? NU_CARGA { get; set; }

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

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SEQ_PREPARACION { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 6)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public int? NU_CONTENEDOR { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_PICKEO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? NU_CONTENEDOR_PICKEO { get; set; }

        public int? CD_FUNC_PICKEO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        public int? NU_CONTENEDOR_SYS { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ERROR_CONTROL { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_FUNCIONARIO { get; set; }

        [StringLength(50)]
        [Column]
        public string LOGINNAME_FUNC { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_FUNC_PICKEO { get; set; }

        [StringLength(50)]
        [Column]
        public string LOGINNAME_PICKEO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO_1 { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_CONTROL { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }



        public short CD_RAMO_PRODUTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_RAMO_PRODUTO { get; set; }

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
        [StringLength(18)]
        [Column]
        public string DS_ANEXO5 { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }

        public DateTime? DT_SEPARACION { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ZONA_UBICACION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ZONA_UBICACION { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ENDERECO_BAIXO { get; set; }

        public decimal? QT_GENERICO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO { get; set; }

        public DateTime? DT_FABRICACAO_PICKEO { get; set; }

        [StringLength(20)]
        public string ND_SECTOR { get; set; }

        [StringLength(100)]
        public string DS_DOMINIO_VALOR { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }

		public int? CD_ROTA { get; set; }

		[StringLength(30)]
		[Column]
		public string DS_ROTA { get; set; }

		public int? CD_TRANSPORTADORA { get; set; }

		[StringLength(100)]
		[Column]
		public string DS_TRANSPORTADORA { get; set; }

		[StringLength(6)]
		[Column]
		public string TP_PEDIDO { get; set; }

		[StringLength(60)]
		[Column]
		public string DS_TIPO_PEDIDO { get; set; }

        public int? NU_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(40)]
        public string NU_EXTERNO_UNIDAD { get; set; }

        [StringLength(10)]
        public string TP_CONTENEDOR { get; set; }

        public int? CD_CAMION { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_CONTENEDOR { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

    }
}
