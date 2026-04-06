using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_AUTOMATISMO")]
    public partial class T_AUTOMATISMO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_AUTOMATISMO()
        {
            T_AUTOMATISMO_POSICION = new HashSet<T_AUTOMATISMO_POSICION>();
            T_AUTOMATISMO_EJECUCION = new HashSet<T_AUTOMATISMO_EJECUCION>();
            T_AUTOMATISMO_INTERFAZ = new HashSet<T_AUTOMATISMO_INTERFAZ>();
            T_AUTOMATISMO_PUESTO = new HashSet<T_AUTOMATISMO_PUESTO>();
            T_AUTOMATISMO_CARACTERISTICA = new HashSet<T_AUTOMATISMO_CARACTERISTICA>();
        }

        [Key]
        [Column(Order = 1)]
        public int NU_AUTOMATISMO { get; set; }

        [Required]
        [StringLength(100)]
        public string CD_AUTOMATISMO { get; set; }

        [StringLength(400)]
        public string DS_AUTOMATISMO { get; set; }

        [Required]
        [StringLength(40)]
        public string ND_TP_AUTOMATISMO { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [Required]
        [StringLength(20)]
        public string CD_ZONA_UBICACION { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }


        public long? NU_TRANSACCION { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_AUTOMATISMO_POSICION> T_AUTOMATISMO_POSICION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_AUTOMATISMO_EJECUCION> T_AUTOMATISMO_EJECUCION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_AUTOMATISMO_INTERFAZ> T_AUTOMATISMO_INTERFAZ { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_AUTOMATISMO_PUESTO> T_AUTOMATISMO_PUESTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_AUTOMATISMO_CARACTERISTICA> T_AUTOMATISMO_CARACTERISTICA { get; set; }
    }
}
