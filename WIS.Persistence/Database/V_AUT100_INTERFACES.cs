
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_AUT100_INTERFACES")]
    public partial class V_AUT100_INTERFACES
    {

        [Key]
        [Column(Order = 0)]
        public int NU_AUTOMATISMO_INTERFAZ { get; set; }

        public int CD_INTERFAZ { get; set; }

        [StringLength(100)]
        public string DS_INTERFAZ { get; set; }

        [StringLength(400)]
        public string VL_URL { get; set; }

        [StringLength(100)]
        public string ND_PROTOCOLO_COMUNICACION { get; set; }

        public int? NU_INTEGRACION_SERVICIO { get; set; }

        public int NU_AUTOMATISMO { get; set; }

        public int CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(100)]
        public string DS_INTERFAZ_EXTERNA { get; set; }

    }
}
