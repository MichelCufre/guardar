using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
 
    [Table("T_COLA_TRABAJO_PONDERADOR")]
    public partial class T_COLA_TRABAJO_PONDERADOR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_COLA_TRABAJO_PONDERADOR()
        {
            T_COLA_TRABAJO_PONDERADOR_DET = new HashSet<T_COLA_TRABAJO_PONDERADOR_DET>();
        }

        [Key]
        public int NU_COLA_TRABAJO { get; set; }

        [Key]
        [StringLength(100)]
        public string CD_PONDERADOR { get; set; }

        public int? NU_INCREMENTO { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual T_COLA_TRABAJO T_COLA_TRABAJO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_COLA_TRABAJO_PONDERADOR_DET> T_COLA_TRABAJO_PONDERADOR_DET { get; set; }
    }
}
