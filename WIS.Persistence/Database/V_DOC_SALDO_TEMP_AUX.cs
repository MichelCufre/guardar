using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOC_SALDO_TEMP_AUX")]
    public partial class V_DOC_SALDO_TEMP_AUX
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        public long? QT_LINEAS { get; set; }

        public long? QT_PRODUCTOS { get; set; }

        public decimal? QT_DESAFECTADA { get; set; }

        public decimal? VL_CIF { get; set; }
    }

}
