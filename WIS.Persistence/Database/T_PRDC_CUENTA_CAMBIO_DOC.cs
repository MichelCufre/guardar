using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_PRDC_CUENTA_CAMBIO_DOC")]
    public partial class T_PRDC_CUENTA_CAMBIO_DOC
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO_EGRESO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string NU_DOCUMENTO_EGRESO_PRDC { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(6)]
        public string TP_DOCUMENTO_EGRESO_PRDC { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO_ORIGINAL { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO_ORIGINAL { get; set; }

        public decimal? QT_DECLARADA_ORIGINAL { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 9)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 10)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        [Key]
        [Column(Order = 11)]
        [StringLength(40)]
        public string CD_PRODUTO_PRODUCIDO { get; set; }

        [Key]
        [Column(Order = 12)]
        public decimal NU_NIVEL { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public DateTime? DT_UPDROW { get; set; }
        public DateTime? DT_ADDROW { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO_CAMBIO { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO_CAMBIO { get; set; }
    }
}
