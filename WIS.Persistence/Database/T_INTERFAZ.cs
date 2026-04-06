namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_INTERFAZ")]
    public partial class T_INTERFAZ
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_INTERFAZ()
        {
            T_INTERFAZ_EXTERNA = new HashSet<T_INTERFAZ_EXTERNA>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_INTERFAZ { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_INTERFAZ { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_INTERFAZ { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ENTRADA_SALIDA { get; set; }

        [StringLength(30)]
        [Column]
        public string TP_OBJETO_BD { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_OBJETO { get; set; }

        [StringLength(60)]
        [Column]
        public string NM_PROCEDIMIENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_IGNORAR_ERROR_CARGA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ESPERAR_APROBACION { get; set; }

        [StringLength(30)]
        [Column]
        public string VL_OBJETO_CONSULTA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(40)]
        public string ND_TIPO_AUTOMATISMO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_INTERFAZ_EXTERNA> T_INTERFAZ_EXTERNA { get; set; }
    }
}
