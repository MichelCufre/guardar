namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_INTERFAZ_EJECUCION")]
    public partial class T_INTERFAZ_EJECUCION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_INTERFAZ_EJECUCION()
        {
            T_INTERFAZ_EJECUCION_ERROR = new HashSet<T_INTERFAZ_EJECUCION_ERROR>();
            T_INTERFAZ_EJECUCION_DATEXTDET = new HashSet<T_INTERFAZ_EJECUCION_DATEXTDET>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        public int? CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_ARCHIVO { get; set; }

        public short? CD_SITUACAO { get; set; }

        public DateTime? DT_COMIENZO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ERROR_CARGA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ERROR_PROCEDIMIENTO { get; set; }

        public int? CD_FUNCIONARIO_ACEPTACION { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_REFERENCIA { get; set; }

        [StringLength(30)]
        [Column]
        public string ND_SITUACION { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_GRUPO_CONSULTA { get; set; }

        public int? USERID { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_REQUEST { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_INTERFAZ_EJECUCION_ERROR> T_INTERFAZ_EJECUCION_ERROR { get; set; }

        public virtual T_INTERFAZ_EXTERNA T_INTERFAZ_EXTERNA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_INTERFAZ_EJECUCION_DATEXTDET> T_INTERFAZ_EJECUCION_DATEXTDET { get; set; }

        public virtual T_INTERFAZ_EJECUCION_DATA T_INTERFAZ_EJECUCION_DATA { get; set; }

        public virtual T_INTERFAZ_EJECUCION_DATEXT T_INTERFAZ_EJECUCION_DATEXT { get; set; }
    }
}
