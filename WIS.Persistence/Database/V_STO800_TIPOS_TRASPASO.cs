using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO800_TIPOS_TRASPASO")]
    public partial class V_STO800_TIPOS_TRASPASO
    {
        [Required]
        [StringLength(20)]
        public string TP_TRASPASO { get; set; }

        [StringLength(100)]
        public string DS_TRASPASO { get; set; }

    }
}