namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_SUB_CLASSE")]
    public partial class T_SUB_CLASSE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_SUB_CLASSE()
        {
            T_CLASSE = new HashSet<T_CLASSE>();
        }

        [Key]
        [StringLength(2)]
        [Column]
        public string CD_SUB_CLASSE { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_SUB_CLASSE { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_CLASSE> T_CLASSE { get; set; }
    }
}
