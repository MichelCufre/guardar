namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_ETIQUETAS_WREC150")]
    public partial class V_ETIQUETAS_WREC150
    {
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
        [StringLength(40)]
        public string NU_EXTERNO_ETIQUETA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(3)]
        public string TP_ETIQUETA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        public short? CD_SITUACAO_DET_AGENDA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO_DET_AGENDA { get; set; }

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

        public DateTime? DT_FABRICACAO { get; set; }

        public decimal? QT_PRODUTO_RECIBIDO { get; set; }

        public decimal? QT_ALMACENADO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_GRUPO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_NAM { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

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

        [StringLength(1)]
        [Column]
        public string ID_MANEJO_IDENTIFICADOR { get; set; }

        public decimal? QT_ETIQUETA_GENERADA { get; set; }

        public short? CD_SITUACAO_AGENDA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO_AGENDA { get; set; }

        public int? CD_FUNC_ALMACENAMIENTO { get; set; }

        [StringLength(50)]
        [Column]
        public string LOGINNAME_ALM { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME_ALM { get; set; }

        public int? CD_FUNC_RECEPCION { get; set; }

        [StringLength(50)]
        [Column]
        public string LOGINNAME_REC { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME_REC { get; set; }

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

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
