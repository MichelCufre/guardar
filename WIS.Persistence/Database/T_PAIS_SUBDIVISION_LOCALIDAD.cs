namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PAIS_SUBDIVISION_LOCALIDAD")]
    public partial class T_PAIS_SUBDIVISION_LOCALIDAD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_PAIS_SUBDIVISION_LOCALIDAD()
        {
            T_CLIENTE = new HashSet<T_CLIENTE>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID_LOCALIDAD { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_LOCALIDAD { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_SUBDIVISION { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string NM_LOCALIDAD { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_IATA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_POSTAL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_CLIENTE> T_CLIENTE { get; set; }

        public virtual T_PAIS_SUBDIVISION T_PAIS_SUBDIVISION { get; set; }
    }
}
