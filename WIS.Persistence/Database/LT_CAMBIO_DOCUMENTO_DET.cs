using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("LT_CAMBIO_DOCUMENTO_DET")]
    public partial class LT_CAMBIO_DOCUMENTO_DET
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
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(10)]
        public string NU_DOCUMENTO_CAMBIO { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(6)]
        public string TP_DOCUMENTO_CAMBIO { get; set; }
    }

}
