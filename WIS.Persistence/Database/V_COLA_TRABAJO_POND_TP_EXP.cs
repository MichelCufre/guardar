
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;

namespace WIS.Persistence.Database
{

    [Table("V_COLA_TRABAJO_POND_TP_EXP")]
    public partial class V_COLA_TRABAJO_POND_TP_EXP
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string TP_EXPEDICION { get; set; }

        [StringLength(60)]
        public string NM_EXPEDICION { get; set; }

        public int? NU_PONDERACION { get; set; }

		public int? NU_PONDERACION_DEFAULT { get; set; }

        public int NU_COLA_TRABAJO { get; set; }

        [StringLength(5)]
        public string VL_OPERACION { get; set; }

    }
}
