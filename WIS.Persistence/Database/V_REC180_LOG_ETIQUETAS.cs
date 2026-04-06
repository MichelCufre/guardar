namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REC180_LOG_ETIQUETAS")]
    public partial class V_REC180_LOG_ETIQUETAS
    {
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ETIQUETA_LOTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(3)]
        public string TP_ETIQUETA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_EXTERNO_ETIQUETA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_NAM { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_NAM { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_UNIDADE_MEDIDA { get; set; }

        public int? CD_FAMILIA_PRODUTO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_LOG { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_APLICACAO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_APLICACAO { get; set; }

        public DateTime? DT_OPERACION { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_LOG_ETIQUETA { get; set; }

        public decimal? CD_FAIXA { get; set; }

        public DateTime? DT_RECEPCION { get; set; }

        public DateTime? DT_ALMACENAMIENTO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME { get; set; }

        [StringLength(50)]
        [Column]
        public string LOGGINNAME { get; set; }

        public int? CD_FUNC_RECEPCION { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME_REC { get; set; }

        [StringLength(50)]
        [Column]
        public string LOGGINNAME_REC { get; set; }

        public int? CD_FUNC_ALMACENAMIENTO { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME_ALM { get; set; }

        [StringLength(50)]
        [Column]
        public string LOGGINNAME_ALM { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_CAB { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_GRUPO { get; set; }

        public short? CD_PALLET { get; set; }
    }
}
