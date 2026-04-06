using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_INTERFAZ_EXTERNA")]
    public partial class T_INTERFAZ_EXTERNA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_INTERFAZ_EXTERNA()
        {
            T_INTERFAZ_EJECUCION = new HashSet<T_INTERFAZ_EJECUCION>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_INTERFAZ_EXTERNA { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_ARCHIVO { get; set; }

        public int CD_INTERFAZ { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_RECONO_PREFIJO { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_RECONO_POSTFIJO { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_RECONO_EXTENSION { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_RECONO_CONTENIDO { get; set; }

        public short? NU_RECONO_ORDEN { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_DELIMITADOR { get; set; }

        [StringLength(60)]
        [Column]
        public string NM_PROCEDIMIENTO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public decimal? LN_COMIENZO_PROCESO { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_DELIMITADOR_SEGMENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string ID_SECUENCIA { get; set; }

        [StringLength(61)]
        [Column]
        public string VL_PROC_EXTRAE_SECUENCIA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_RE_PROCESABLE { get; set; }

        [StringLength(100)]
        public string VL_ENDPOINT { get; set; }

        [StringLength(100)]
        public string VL_ENDPOINT_REPROCESS { get; set; }

        [StringLength(100)]
        public string VL_PARAMETRO_HABILITACION { get; set; }

        public virtual T_INTERFAZ T_INTERFAZ { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_INTERFAZ_EJECUCION> T_INTERFAZ_EJECUCION { get; set; }
    }
}
