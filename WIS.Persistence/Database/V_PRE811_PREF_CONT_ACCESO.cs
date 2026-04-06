using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_CONT_ACCESO")]
    public partial class V_PRE811_PREF_CONT_ACCESO
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string CD_CONTROL_ACCESO { get; set; }

        [Required]
        [StringLength(100)]
        public string DS_CONTROL_ACCESO { get; set; }
        public int? NU_PREFERENCIA_CONT_ACCESO { get; set; }

    }
}