using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_ROTA")]
    public partial class V_PRE811_PREF_ROTA
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        public short CD_ROTA { get; set; }

        [StringLength(30)]
        public string DS_ROTA { get; set; }

        public int? NU_PREFERENCIA_ROTA { get; set; }

    }
}