using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_COLA_TRABAJO")]
    public partial class T_COLA_TRABAJO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_COLA_TRABAJO()
        {
            T_COLA_TRABAJO_PONDERADOR = new HashSet<T_COLA_TRABAJO_PONDERADOR>();
            T_COLA_TRABAJO_PONDERADOR_DET = new HashSet<T_COLA_TRABAJO_PONDERADOR_DET>();
        }

        [Key]
        public int NU_COLA_TRABAJO { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(200)]
        public string DS_COLA_TRABAJO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
        [StringLength(2)]
        public string FL_ORDEN_CALENDARIO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_COLA_TRABAJO_PONDERADOR> T_COLA_TRABAJO_PONDERADOR { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_COLA_TRABAJO_PONDERADOR_DET> T_COLA_TRABAJO_PONDERADOR_DET { get; set; }

    }
}
