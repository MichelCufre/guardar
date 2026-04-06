namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PAIS")]
    public partial class T_PAIS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_PAIS()
        {
            T_PAIS_SUBDIVISION = new HashSet<T_PAIS_SUBDIVISION>();
        }

        [Key]
        [StringLength(2)]
        [Column]
        public string CD_PAIS { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PAIS { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_PAIS_ALFA3 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PAIS_SUBDIVISION> T_PAIS_SUBDIVISION { get; set; }
    }
}
