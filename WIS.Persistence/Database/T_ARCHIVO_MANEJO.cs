namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ARCHIVO_MANEJO")]
    public partial class T_ARCHIVO_MANEJO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_ARCHIVO_MANEJO()
        {
            T_ARCHIVO_ADJUNTO = new HashSet<T_ARCHIVO_ADJUNTO>();
            T_ARCHIVO_MANEJO_DOCUMENTO = new HashSet<T_ARCHIVO_MANEJO_DOCUMENTO>();
        }

        [Key]
        [StringLength(5)]
        [Column]
        public string CD_MANEJO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_MANEJO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(300)]
        [Column]
        public string CD_ANEXOS { get; set; }

        [StringLength(300)]
        [Column]
        public string DS_ANEXOS { get; set; }

        [StringLength(300)]
        [Column]
        public string CD_CAMPOS { get; set; }

        [StringLength(300)]
        [Column]
        public string DS_CAMPOS { get; set; }

        [StringLength(30)]
        [Column]
        public string SUB_LINK { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ARCHIVO_ADJUNTO> T_ARCHIVO_ADJUNTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ARCHIVO_MANEJO_DOCUMENTO> T_ARCHIVO_MANEJO_DOCUMENTO { get; set; }
    }
}
