using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TRASPASO_CONF_TIPO_TRASPASO")]
    public partial class T_TRASPASO_CONF_TIPO_TRASPASO
    {
        public long NU_TRASPASO_CONF_TIPO_TRASPASO { get; set; }

        public long NU_TRASPASO_CONFIGURACION { get; set; }

        [Required]
        [StringLength(50)]
        public string TP_TRASPASO { get; set; }

    }
}