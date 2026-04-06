using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_EQUIPO")]
    public partial class V_PRE811_PREF_EQUIPO
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        public int CD_EQUIPO { get; set; }

        [Required]
        [StringLength(40)]
        public string DS_EQUIPO { get; set; }
        public int? NU_PREFERENCIA_EQUIPO { get; set; }

    }
}