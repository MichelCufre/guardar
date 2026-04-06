namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DET_PEDIDO_SAIDA_WPRE101")]
    public partial class V_DET_PEDIDO_SAIDA_WPRE101
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

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

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

        [StringLength(30)]
        [Column]
        public string CD_UNIDADE_MEDIDA { get; set; }

        public decimal? QT_UND_BULTO { get; set; }

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

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        public decimal? QT_PEDIDO_ORIGINAL { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? VL_PORCENTAJE_TOLERANCIA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_MEMO { get; set; }

        public decimal? QT_CONV { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
