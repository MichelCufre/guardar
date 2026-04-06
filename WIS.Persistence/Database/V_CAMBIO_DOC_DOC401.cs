using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_CAMBIO_DOC_DOC401")]
    public partial class V_CAMBIO_DOC_DOC401
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESSO { get; set; }

        public decimal? QT_INGRESADA { get; set; }

        public decimal? QT_RESERVADA { get; set; }

        public decimal? QT_DESAFECTADA { get; set; }

        public decimal? QT_SALDO { get; set; }

        [StringLength(10)]
        public string NU_DOC { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        public decimal? QT_NACIONALIZADA { get; set; }

        public decimal? QT_EXPEDIDO { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO_CAMBIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }

}
