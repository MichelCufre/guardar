namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("T_ALM_ESTRATEGIA")]
    public partial class T_ALM_ESTRATEGIA
    {
        public T_ALM_ESTRATEGIA()
        {
            this.T_ALM_SUGERENCIA = new HashSet<T_ALM_SUGERENCIA>();
        }

        [Key]
        public int NU_ALM_ESTRATEGIA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ALM_ESTRATEGIA { get; set; }
        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        [StringLength(64)]
        [Column]
        public string VL_AUDITORIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ALM_SUGERENCIA> T_ALM_SUGERENCIA { get; set; }
    }
}
