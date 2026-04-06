namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_INSTANCIA")]
    public partial class T_EVENTO_INSTANCIA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_EVENTO_INSTANCIA()
        {
            T_EVENTO_BANDEJA_INSTANCIA = new HashSet<T_EVENTO_BANDEJA_INSTANCIA>();
            T_EVENTO_GRUPO_INSTANCIA_REL = new HashSet<T_EVENTO_GRUPO_INSTANCIA_REL>();
            T_EVENTO_NOTIFICACION = new HashSet<T_EVENTO_NOTIFICACION>();
            T_EVENTO_PARAMETRO_INSTANCIA = new HashSet<T_EVENTO_PARAMETRO_INSTANCIA>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO_INSTANCIA { get; set; }

        public int NU_EVENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_INSTANCIA { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_HABILITADO { get; set; }

        [StringLength(60)]
        [Column]
        public string TP_NOTIFICACION { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_LABEL_ESTILO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public virtual T_EVENTO T_EVENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_BANDEJA_INSTANCIA> T_EVENTO_BANDEJA_INSTANCIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_GRUPO_INSTANCIA_REL> T_EVENTO_GRUPO_INSTANCIA_REL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_NOTIFICACION> T_EVENTO_NOTIFICACION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_PARAMETRO_INSTANCIA> T_EVENTO_PARAMETRO_INSTANCIA { get; set; }
        
        public virtual T_EVENTO_TEMPLATE T_EVENTO_TEMPLATE { get; set; }
    }
}
