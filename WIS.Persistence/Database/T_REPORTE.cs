namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_REPORTE")]
    public partial class T_REPORTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_REPORTE()
        {
            T_REPORTE_RELACION = new HashSet<T_REPORTE_RELACION>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_REPORTE { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_REPORTE { get; set; }

        public byte[] VL_DATA { get; set; }

        public int? CD_USUARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_ARCHIVO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_SITUACION { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ZONA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_REPORTE_RELACION> T_REPORTE_RELACION { get; set; }
    }
}
