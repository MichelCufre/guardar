namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOMINIO")]
    public partial class T_DOMINIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_DOMINIO()
        {
            T_DET_DOMINIO = new HashSet<T_DET_DOMINIO>();
        }

        [Key]
        [StringLength(10)]
        [Column]
        public string CD_DOMINIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_INTERNO_WIS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_DOMINIO> T_DET_DOMINIO { get; set; }
    }
}
