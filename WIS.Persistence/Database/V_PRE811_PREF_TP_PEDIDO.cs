using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PRE811_PREF_TP_PEDIDO")]
    public partial class V_PRE811_PREF_TP_PEDIDO
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_PEDIDO { get; set; }

        [Required]
        [StringLength(60)]
        public string DS_TIPO_PEDIDO { get; set; }

        public int? NU_PREFERENCIA_TP_PEDIDO { get; set; }

    }
}