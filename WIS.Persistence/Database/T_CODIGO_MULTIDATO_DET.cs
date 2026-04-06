using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_CODIGO_MULTIDATO_DET")]
    public partial class T_CODIGO_MULTIDATO_DET
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string CD_CODIGO_MULTIDATO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string CD_AI { get; set; }

        [StringLength(100)]
        public string DS_AI { get; set; }

        [StringLength(100)]
        public string TP_AI { get; set; }

        public decimal? VL_CONVERSION { get; set; }
    }
}
