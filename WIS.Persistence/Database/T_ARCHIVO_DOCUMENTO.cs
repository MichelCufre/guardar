namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ARCHIVO_DOCUMENTO")]
    public partial class T_ARCHIVO_DOCUMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_ARCHIVO_DOCUMENTO()
        {
            T_ARCHIVO_MANEJO_DOCUMENTO = new HashSet<T_ARCHIVO_MANEJO_DOCUMENTO>();
        }

        [Key]
        [StringLength(5)]
        [Column]
        public string CD_DOCUMENTO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_DOCUMENTO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ARCHIVO_MANEJO_DOCUMENTO> T_ARCHIVO_MANEJO_DOCUMENTO { get; set; }
    }
}
