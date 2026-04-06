namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_BANDEJA")]
    public partial class T_EVENTO_BANDEJA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_EVENTO_BANDEJA()
        {
            T_EVENTO_BANDEJA_INSTANCIA = new HashSet<T_EVENTO_BANDEJA_INSTANCIA>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO_BANDEJA { get; set; }

        public int NU_EVENTO { get; set; }

        [StringLength(2000)]
        [Column]
        public string VL_SEREALIZADO { get; set; }

        [StringLength(100)]
        [Column]
        public string ND_ESTADO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual T_EVENTO T_EVENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_BANDEJA_INSTANCIA> T_EVENTO_BANDEJA_INSTANCIA { get; set; }
    }
}
