using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_AUTOMATISMO_EJECUCION")]
    public partial class T_AUTOMATISMO_EJECUCION
    {

        [Key]
        [Column(Order = 0)]
        public int NU_AUTOMATISMO_EJECUCION { get; set; }

        public int? NU_AUTOMATISMO { get; set; }

        public int? NU_AUTOMATISMO_INTERFAZ { get; set; }

        public int CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(40)]
        public string ND_SITUACION { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(200)]
        public string DS_REFERENCIA { get; set; }

        [StringLength(200)]
        public string VL_IDENTITY_USER { get; set; }

        public virtual T_AUTOMATISMO T_AUTOMATISMO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_AUTOMATISMO_DATA> T_AUTOMATISMO_DATA { get; set; }

    }
}
