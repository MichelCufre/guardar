namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PAIS_SUBDIVISION")]
    public partial class T_PAIS_SUBDIVISION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_PAIS_SUBDIVISION()
        {
            T_PAIS_SUBDIVISION_LOCALIDAD = new HashSet<T_PAIS_SUBDIVISION_LOCALIDAD>();
        }

        [Key]
        [StringLength(20)]
        [Column]
        public string CD_SUBDIVISION { get; set; }

        [Required]
        [StringLength(2)]
        [Column]
        public string CD_PAIS { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string NM_SUBDIVISION { get; set; }

        public virtual T_PAIS T_PAIS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PAIS_SUBDIVISION_LOCALIDAD> T_PAIS_SUBDIVISION_LOCALIDAD { get; set; }
    }
}
