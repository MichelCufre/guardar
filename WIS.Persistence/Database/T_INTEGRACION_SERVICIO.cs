using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_INTEGRACION_SERVICIO")]
    public partial class T_INTEGRACION_SERVICIO
    {

        [Key]
        [Column(Order = 0)]
        public int NU_INTEGRACION { get; set; }

        [Required]
        [StringLength(300)]
        public string CD_INTEGRACION { get; set; }

        [Required]
        [StringLength(300)]
        public string DS_INTEGRACION { get; set; }

        [StringLength(300)]
        public string VL_URL_INTEGRACION { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO { get; set; }

        [StringLength(40)]
        public string ND_TP_AUTH_SRV { get; set; }

        [StringLength(40)]
        public string ND_TP_COMUNICACION { get; set; }

        [StringLength(100)]
        public string VL_USER { get; set; }

        [StringLength(4000)]
        public string VL_SECRET { get; set; }

        [StringLength(4000)]
        public string VL_SECRETSALT { get; set; }

        public decimal? VL_SECRETFORMAT { get; set; }

        [StringLength(100)]
        public string VL_SCOPE { get; set; }

        [StringLength(100)]
        public string VL_URL_AUTH_SERVER { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_AUTOMATISMO_INTERFAZ> T_AUTOMATISMO_INTERFAZ { get; set; }
    }
}
