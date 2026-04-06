using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_COND_LIB")]
    public partial class V_PRE811_PREF_COND_LIB
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string CD_CONDICION_LIBERACION { get; set; }

        [StringLength(60)]
        public string DS_CONDICION_LIBERACION { get; set; }
        public int? NU_PREFERENCIA_COND_LIB { get; set; }

    }
}