namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_REPORTE_DEFINICION")]
    public partial class T_REPORTE_DEFINICION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_REPORTE_DEFINICION()
        {
            T_RECEPCION_TIPO_REPORTE_DEF = new HashSet<T_RECEPCION_TIPO_REPORTE_DEF>();
        }

        [Key]
        [StringLength(30)]
        [Column]
        public string CD_REPORTE { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_REPORTE { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_REPORTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_RECURSO_TEXTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEPCION_TIPO_REPORTE_DEF> T_RECEPCION_TIPO_REPORTE_DEF { get; set; }
    }
}
