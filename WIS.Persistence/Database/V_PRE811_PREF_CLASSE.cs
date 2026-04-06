using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_CLASSE")]
    public partial class V_PRE811_PREF_CLASSE
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        [Required]
        [StringLength(100)]
        public string NM_CLASSE { get; set; }

        public int? NU_PREFERENCIA_CLASSE { get; set; }

    }
}
