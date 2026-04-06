using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_USUARIO")]
    public partial class V_PRE811_PREF_USUARIO
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        public int USERID { get; set; }

        [Required]
        [StringLength(50)]
        public string LOGINNAME { get; set; }

        [Required]
        [StringLength(100)]
        public string FULLNAME { get; set; }

        [StringLength(100)]
        public string EMAIL { get; set; }
        public int? NU_PREFERENCIA_USUARIO { get; set; }

    }
}