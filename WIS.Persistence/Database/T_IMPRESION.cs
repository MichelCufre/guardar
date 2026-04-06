namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_IMPRESION")]
    public partial class T_IMPRESION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_IMPRESION()
        {
            this.T_DET_IMPRESION = new HashSet<T_DET_IMPRESION>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_IMPRESION { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(150)]
        [Column]
        public string DS_REFERENCIA { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_IMPRESORA { get; set; }

        public DateTime? DT_GENERADO { get; set; }

        public int? QT_REGISTROS { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ESTADO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public DateTime? DT_PROCESADO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ERROR { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_LABEL { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_IMPRESORA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_IMPRESION> T_DET_IMPRESION { get; set; }
    }
}
