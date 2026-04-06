namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("T_ALM_LOGICA_INSTANCIA")]
    public partial class T_ALM_LOGICA_INSTANCIA
    {
        public T_ALM_LOGICA_INSTANCIA()
        {
            T_ALM_LOGICA_INSTANCIA_PARAM = new HashSet<T_ALM_LOGICA_INSTANCIA_PARAM>();
            T_ALM_SUGERENCIA = new HashSet<T_ALM_SUGERENCIA>();
            T_ALM_SUGERENCIA_DET = new HashSet<T_ALM_SUGERENCIA_DET>();
        }

        [Key]
        public int NU_ALM_LOGICA_INSTANCIA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ALM_LOGICA_INSTANCIA { get; set; }
        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ORDEN_ASC { get; set; }
        public int NU_ALM_ESTRATEGIA { get; set; }
        public short NU_ALM_LOGICA { get; set; }
        public short NU_ORDEN { get; set; }

        [StringLength(64)]
        [Column]
        public string VL_AUDITORIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ALM_LOGICA_INSTANCIA_PARAM> T_ALM_LOGICA_INSTANCIA_PARAM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ALM_SUGERENCIA> T_ALM_SUGERENCIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ALM_SUGERENCIA_DET> T_ALM_SUGERENCIA_DET { get; set; }
    }
}
