namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CLASSE")]
    public partial class T_CLASSE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_CLASSE()
        {
            T_PRODUTO = new HashSet<T_PRODUTO>();
        }

        [Key]
        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_CLASSE { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_SUB_CLASSE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRODUTO> T_PRODUTO { get; set; }

        public virtual T_SUB_CLASSE T_SUB_CLASSE { get; set; }
    }
}
