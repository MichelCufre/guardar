using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO")]
    public partial class T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string TP_REFERENCIA_EXTERNA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        public virtual T_TIPO_REFERENCIA_EXTERNA T_TIPO_REFERENCIA_EXTERNA { get; set; }

        public virtual T_DOCUMENTO_TIPO T_DOCUMENTO_TIPO { get; set; }
    }
}
