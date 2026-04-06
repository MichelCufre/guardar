using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_FAMILIA")]
    public partial class V_PRE811_PREF_FAMILIA
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        public int CD_FAMILIA_PRODUTO { get; set; }

        [Required]
        [StringLength(100)]
        public string DS_FAMILIA_PRODUTO { get; set; }

        public int? NU_PREFERENCIA_FAMILIA { get; set; }

    }
}