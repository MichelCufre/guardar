namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_GRUPO_CONSULTA")]
    public partial class T_GRUPO_CONSULTA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_GRUPO_CONSULTA()
        {
            T_GRUPO_CONSULTA_FUNCIONARIO = new HashSet<T_GRUPO_CONSULTA_FUNCIONARIO>();
        }

        [Key]
        [StringLength(20)]
        [Column]
        public string CD_GRUPO_CONSULTA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_GRUPO_CONSULTA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_GRUPO_CONSULTA_FUNCIONARIO> T_GRUPO_CONSULTA_FUNCIONARIO { get; set; }
    }
}
