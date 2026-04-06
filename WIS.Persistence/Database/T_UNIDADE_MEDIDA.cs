namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_UNIDADE_MEDIDA")]
    public partial class T_UNIDADE_MEDIDA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_UNIDADE_MEDIDA()
        {
            T_PRODUTO = new HashSet<T_PRODUTO>();
        }

        [Key]
        [StringLength(10)]
        [Column]
        public string CD_UNIDADE_MEDIDA { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_UNIDADE_MEDIDA { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FG_ACEITA_DECIMAL { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_UNIDAD_MEDIDA_EXTERNA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRODUTO> T_PRODUTO { get; set; }
    }
}
