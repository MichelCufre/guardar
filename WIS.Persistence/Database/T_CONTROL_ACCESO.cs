namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CONTROL_ACCESO")]
    public partial class T_CONTROL_ACCESO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_CONTROL_ACCESO()
        {
            T_ENDERECO_ESTOQUE = new HashSet<T_ENDERECO_ESTOQUE>();
        }

        [Key]
        [StringLength(20)]
        [Column]
        public string CD_CONTROL_ACCESO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_CONTROL_ACCESO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ENDERECO_ESTOQUE> T_ENDERECO_ESTOQUE { get; set; }
    }
}
