using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TIPO_REFERENCIA_EXTERNA")]
    public partial class T_TIPO_REFERENCIA_EXTERNA
    {
        [Key]
        [StringLength(6)]
        public string TP_REFERENCIA_EXTERNA { get; set; }

        [StringLength(30)]
        public string DS_REFERENCIA_EXTERNA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO> T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO { get; set; }
    }
}
