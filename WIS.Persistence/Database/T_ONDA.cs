namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ONDA")]
    public partial class T_ONDA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_ONDA()
        {
            T_ROTA = new HashSet<T_ROTA>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_ONDA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ONDA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public DateTime? DT_ALTERACAO { get; set; }

        public DateTime? DT_CADASTRAMENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ROTA> T_ROTA { get; set; }
    }
}
