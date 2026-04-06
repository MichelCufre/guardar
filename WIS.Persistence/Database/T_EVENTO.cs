namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO")]
    public partial class T_EVENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_EVENTO()
        {
            T_EVENTO_BANDEJA = new HashSet<T_EVENTO_BANDEJA>();
            T_EVENTO_INSTANCIA = new HashSet<T_EVENTO_INSTANCIA>();
            T_EVENTO_PARAMETRO_INSTANCIA = new HashSet<T_EVENTO_PARAMETRO_INSTANCIA>();
            T_EVENTO_PARAMETRO = new HashSet<T_EVENTO_PARAMETRO>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_EVENTO { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_PROGRAMADO { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_EVENTO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_BANDEJA> T_EVENTO_BANDEJA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_INSTANCIA> T_EVENTO_INSTANCIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_PARAMETRO_INSTANCIA> T_EVENTO_PARAMETRO_INSTANCIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_PARAMETRO> T_EVENTO_PARAMETRO { get; set; }
    }
}
