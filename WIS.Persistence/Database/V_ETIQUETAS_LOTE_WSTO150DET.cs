namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_ETIQUETAS_LOTE_WSTO150DET")]
    public partial class V_ETIQUETAS_LOTE_WSTO150DET
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

        public short? CD_SITUACAO_AGENDA { get; set; }

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

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        public decimal? QT_ETIQUETA_GENERADA { get; set; }

        public short? CD_SIT_CAB_AGENDA { get; set; }
    }
}
