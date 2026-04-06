namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_LPARAMETRO_NIVEL")]
    public partial class T_LPARAMETRO_NIVEL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_LPARAMETRO_NIVEL()
        {
            T_LPARAMETRO_CONFIGURACION = new HashSet<T_LPARAMETRO_CONFIGURACION>();
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string CD_PARAMETRO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string DO_ENTIDAD_PARAMETRIZABLE { get; set; }

        public short? NU_NIVEL { get; set; }

        public virtual T_LPARAMETRO T_LPARAMETRO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LPARAMETRO_CONFIGURACION> T_LPARAMETRO_CONFIGURACION { get; set; }
    }
}
