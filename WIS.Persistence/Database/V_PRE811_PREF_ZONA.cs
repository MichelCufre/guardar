using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_ZONA")]
    public partial class V_PRE811_PREF_ZONA
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string CD_ZONA { get; set; }

        [Required]
        [StringLength(100)]
        public string NM_ZONA { get; set; }
        public int? NU_PREFERENCIA_ZONA { get; set; }

    }
}