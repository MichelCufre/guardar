using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_DOCUMENTO_AGRUPADOR_TIPO")]
    public partial class T_DOCUMENTO_AGRUPADOR_TIPO
    {
        public T_DOCUMENTO_AGRUPADOR_TIPO()
        {
            T_DOCUMENTO_AGRUPADOR = new HashSet<T_DOCUMENTO_AGRUPADOR>();
            T_DOCUMENTO_AGRUPADOR_GRUPO = new HashSet<T_DOCUMENTO_AGRUPADOR_GRUPO>();
        }

        [Key]
        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        [StringLength(64)]
        public string DS_TIPO { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO { get; set; }

        [StringLength(1)]
        public string TP_OPERACION { get; set; }

        public int? QT_DOCUMENTO { get; set; }

        [StringLength(1)]
        public string FL_MANEJA_PREDIO { get; set; }

        [StringLength(50)]
        public string NM_SECUENCIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_AGRUPADOR> T_DOCUMENTO_AGRUPADOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_AGRUPADOR_GRUPO> T_DOCUMENTO_AGRUPADOR_GRUPO { get; set; }
    }
}
