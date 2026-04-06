namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_LPARAMETRO")]
    public partial class T_LPARAMETRO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_LPARAMETRO()
        {
            T_LPARAMETRO_NIVEL = new HashSet<T_LPARAMETRO_NIVEL>();
        }

        [Key]
        [StringLength(30)]
        [Column]
        public string CD_PARAMETRO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_PARAMETRO { get; set; }

        [StringLength(30)]
        [Column]
        public string VL_PROCEDIMIENTO_VALIDACION { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_EXPRESION_REGULAR_VALIDACIO { get; set; }

        [StringLength(10)]
        [Column]
        public string DO_VALOR_PARAMETRO { get; set; }

        [StringLength(1)]
        public string FL_PERMITE_EDICION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LPARAMETRO_NIVEL> T_LPARAMETRO_NIVEL { get; set; }
    }
}
