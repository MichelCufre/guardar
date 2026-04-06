using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_AUTOMATISMO_INTERFAZ")]
    public partial class T_AUTOMATISMO_INTERFAZ
    {

        [Key]
        [Column(Order = 0)]
        public int NU_AUTOMATISMO_INTERFAZ { get; set; }

        public int? NU_INTEGRACION { get; set; }

        public int NU_AUTOMATISMO { get; set; }

        public int CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(400)]
        public string VL_METHOD { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public int? CD_INTERFAZ { get; set; }

        [StringLength(40)]
        public string ND_PROTOCOLO_COMUNICACION { get; set; }

        public virtual T_INTEGRACION_SERVICIO T_INTEGRACION_SERVICIO { get; set; }

        public virtual T_AUTOMATISMO T_AUTOMATISMO { get; set; }

    }
}
