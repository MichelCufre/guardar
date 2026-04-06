using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TIPO_DUA_DOCUMENTO")]
    public partial class T_TIPO_DUA_DOCUMENTO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string TP_DUA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        public virtual T_TIPO_DUA T_TIPO_DUA { get; set; }

        public virtual T_DOCUMENTO_TIPO T_DOCUMENTO_TIPO { get; set; }
    }
}
