using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_AUTOMATISMO_CARACTERISTICA_CONFIG")]
    public partial class T_AUTOMATISMO_CARACTERISTICA_CONFIG
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string TP_AUTOMATISMO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string CD_AUTOMATISMO_CARACTERISTICA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(400)]
        public string VL_AUTOMATISMO_CARACTERISTICA { get; set; }

        [StringLength(400)]
        public string DS_AUTOMATISMO_CARACTERISTICA { get; set; }

        [StringLength(100)]
        public string VL_AUX1 { get; set; }

        public long? NU_AUX1 { get; set; }

        public decimal? QT_AUX1 { get; set; }

        [StringLength(1)]
        public string FL_AUX1 { get; set; }

        [StringLength(4000)]
        public string VL_OPCIONES { get; set; }

    }
}
