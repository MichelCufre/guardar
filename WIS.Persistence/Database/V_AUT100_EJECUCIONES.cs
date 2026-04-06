
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_AUT100_EJECUCIONES")]
    public partial class V_AUT100_EJECUCIONES
    {

        [Key]
        [Column(Order = 0)]
        public int NU_AUTOMATISMO_EJECUCION { get; set; }

        public int? NU_AUTOMATISMO { get; set; }

        public int? NU_AUTOMATISMO_INTERFAZ { get; set; }

        [StringLength(400)]
        public string VL_METHOD { get; set; }

        [StringLength(40)]
        public string ND_PROTOCOLO_COMUNICACION { get; set; }

        public int CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(100)]
        public string DS_INTERFAZ_EXTERNA { get; set; }

        [StringLength(40)]
        public string ND_SITUACION { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        [StringLength(200)]
        public string DS_REFERENCIA { get; set; }

        [StringLength(200)]
        public string VL_IDENTITY_USER { get; set; }

    }
}
