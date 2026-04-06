using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PRDC_SALDO_INGRESADO")]
    public partial class V_PRDC_SALDO_INGRESADO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO_EGR { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO_EGR { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string NU_DOCUMENTO_ING { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(6)]
        public string TP_DOCUMENTO_ING { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string NU_PRDC_INGRESO { get; set; }

        public decimal? QT_INGRESADO { get; set; }
    }
}
