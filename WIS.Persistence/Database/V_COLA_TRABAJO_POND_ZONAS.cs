using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_COLA_TRABAJO_POND_ZONAS")]
    public partial class V_COLA_TRABAJO_POND_ZONAS
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CD_ZONA { get; set; }

        [Required]
        [StringLength(100)]
        public string NM_ZONA { get; set; }

        [StringLength(200)]
        public string DS_ZONA { get; set; }

        public int? NU_PONDERACION { get; set; }

		public int? NU_PONDERACION_DEFAULT { get; set; }

        public int NU_COLA_TRABAJO { get; set; }

        [StringLength(5)]
        public string VL_OPERACION { get; set; }

    }
}