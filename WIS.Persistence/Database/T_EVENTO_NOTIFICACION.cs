namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_NOTIFICACION")]
    public partial class T_EVENTO_NOTIFICACION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_EVENTO_NOTIFICACION()
        {
            T_EVENTO_NOTIFICACION_ARCHIVO = new HashSet<T_EVENTO_NOTIFICACION_ARCHIVO>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_EVENTO_NOTIFICACION { get; set; }

        public int NU_EVENTO_INSTANCIA { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual T_EVENTO_INSTANCIA T_EVENTO_INSTANCIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_NOTIFICACION_ARCHIVO> T_EVENTO_NOTIFICACION_ARCHIVO { get; set; }

        public virtual T_EVENTO_NOTIFICACION_EMAIL T_EVENTO_NOTIFICACION_EMAIL { get; set; }
    }
}
