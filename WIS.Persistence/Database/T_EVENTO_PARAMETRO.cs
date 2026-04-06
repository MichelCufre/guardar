namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_PARAMETRO")]
    public partial class T_EVENTO_PARAMETRO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_EVENTO_PARAMETRO()
        {
            T_EVENTO_PARAMETRO_INSTANCIA = new HashSet<T_EVENTO_PARAMETRO_INSTANCIA>();
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string CD_EVENTO_PARAMETRO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_EVENTO_PARAMETRO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(60)]
        public string TP_NOTIFICACION { get; set; }

        [StringLength(30)]
        [Column]
        public string TP_PARAMETRO { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_EXPRESION_REGULAR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_REQUERIDO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual T_EVENTO T_EVENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_PARAMETRO_INSTANCIA> T_EVENTO_PARAMETRO_INSTANCIA { get; set; }
    }
}
