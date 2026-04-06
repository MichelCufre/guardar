using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TIPO_CONTENEDOR")]
    public partial class T_TIPO_CONTENEDOR
    {
        public T_TIPO_CONTENEDOR()
        {
            T_REL_LABELESTILO_TIPOCONT = new HashSet<T_REL_LABELESTILO_TIPOCONT>();
        }

        [Key]
        [StringLength(10)]
        [Column]
        public string TP_CONTENEDOR { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string DS_TIPO_CONTENEDOR { get; set; }

        public int VL_RANGO_INICIAL { get; set; }

        public int VL_RANGO_FINAL { get; set; }

        public int? VL_ULTIMA_SECUENCIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CLIENTE_PREDEFINIDO { get; set; }

        [StringLength(10)]
        [Column]
        public string ND_TP_ENVASE { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_RETORNABLE { get; set; }
        
        [StringLength(10)]
        [Column]
        public string TP_OBJETO_TRACKING { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO{ get; set; }

        [StringLength(30)]
        public string NM_SECUENCIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_REL_LABELESTILO_TIPOCONT> T_REL_LABELESTILO_TIPOCONT { get; set; }
    }
}
