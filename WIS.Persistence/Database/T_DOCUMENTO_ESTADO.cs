namespace WIS.Persistence.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOCUMENTO_ESTADO")]
    public partial class T_DOCUMENTO_ESTADO
    {
        public T_DOCUMENTO_ESTADO()
        {
            T_DOCUMENTO_ESTADO_ORDEN_ORIGEN = new HashSet<T_DOCUMENTO_ESTADO_ORDEN>();
            T_DOCUMENTO_ESTADO_ORDEN_DESTINO = new HashSet<T_DOCUMENTO_ESTADO_ORDEN>();
            T_DOCUMENTO_TIPO_ESTADO = new HashSet<T_DOCUMENTO_TIPO_ESTADO>();
        }

        [Key]
        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_ESTADO_ORDEN> T_DOCUMENTO_ESTADO_ORDEN_ORIGEN { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_ESTADO_ORDEN> T_DOCUMENTO_ESTADO_ORDEN_DESTINO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_TIPO_ESTADO> T_DOCUMENTO_TIPO_ESTADO { get; set; }
    }
}
