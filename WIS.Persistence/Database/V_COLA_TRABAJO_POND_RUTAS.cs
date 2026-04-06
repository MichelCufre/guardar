using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_COLA_TRABAJO_POND_RUTAS")]
    public partial class V_COLA_TRABAJO_POND_RUTAS
    {

        [Key]
        [Column(Order = 0)]
        public short CD_ROTA { get; set; }

        [StringLength(30)]
        public string DS_ROTA { get; set; }

        public int? NU_PONDERACION { get; set; }

		public int? NU_PONDERACION_DEFAULT { get; set; }

        public int NU_COLA_TRABAJO { get; set; }

        [StringLength(5)]
        public string VL_OPERACION { get; set; }

    }
}